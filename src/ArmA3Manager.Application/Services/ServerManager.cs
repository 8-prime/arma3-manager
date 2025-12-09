using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Channels;
using ArmA3Manager.Application.Common.Builder;
using ArmA3Manager.Application.Common.Constants;
using ArmA3Manager.Application.Common.DataTypes;
using ArmA3Manager.Application.Common.Enums;
using ArmA3Manager.Application.Common.Interfaces;
using ArmA3Manager.Application.Common.Models;
using ArmA3Manager.Application.Common.Models.Server;
using CliWrap;
using CliWrap.EventStream;
using Microsoft.Extensions.Options;

namespace ArmA3Manager.Application.Services;

public partial class ServerManager : IServerManager
{
    private static readonly Regex AnsiRegex = GeneratedAnsiRegex();
    private readonly IUpdatesQueue<ServerLogEntry> _updatesQueue;
    private UpdateOperation? _updateTask;
    private readonly string _steamCmdPath;
    private readonly string _armaServerPath;
    private readonly string _serverDir;
    private readonly string _configFilePath;
    private readonly bool _autoStartServer;
    private readonly ManagerSettings _settings;
    private readonly RingBuffer<ServerLogEntry> _serverLogBuffer;
    private readonly IConfigManager _configManager;
    private ServerStatus _serverStatus = ServerStatus.NotInitialized;
    private DateTime? _runningSince;

    private Task? _serverTask;
    private CancellationTokenSource? _serverCts;

    public ServerManager(IOptions<ManagerSettings> managerSettings, IUpdatesQueue<ServerLogEntry> updatesQueue,
        IConfigManager configManager)
    {
        _serverLogBuffer = new RingBuffer<ServerLogEntry>(500);
        _updatesQueue = updatesQueue;
        _configManager = configManager;
        _configManager.OnConfigurationChanged += WriteActiveConfig;
        _settings = managerSettings.Value;
        _steamCmdPath = managerSettings.Value.SteamCmdPath;
        _armaServerPath = managerSettings.Value.ArmaServerPath;
        _serverDir = managerSettings.Value.ServerDir;
        _configFilePath = managerSettings.Value.ConfigPath;
        _autoStartServer = managerSettings.Value.AutoStartServer;
    }

    public void StartServer()
    {
        if (!File.Exists(_armaServerPath))
        {
            return;
        }

        if (_updateTask is not null)
            return;

        if (_serverTask is not null && !_serverTask.IsCompleted)
        {
            Console.WriteLine("Server is already running.");
            return;
        }

        Console.WriteLine("Starting Arma 3 Server...");

        _serverCts = new CancellationTokenSource();

        var cmd = Cli.Wrap(_armaServerPath)
            .WithArguments(
                string.Join(' ', $"-config={_configFilePath}", _configManager.ActiveConfig?.LaunchParameters))
            .WithWorkingDirectory(_serverDir);

        _serverTask = Task.Run(async () =>
        {
            try
            {
                await foreach (var ev in cmd.ListenAsync(_serverCts.Token))
                {
                    switch (ev)
                    {
                        case StandardOutputCommandEvent stdout:
                            _serverLogBuffer.Add(new ServerLogEntry
                            {
                                Message = stdout.Text,
                                Severity = ServerLogSeverity.Info,
                                Timestamp = DateTime.UtcNow,
                            });
                            Console.WriteLine($"[Server] {stdout.Text}");
                            break;

                        case StandardErrorCommandEvent stderr:
                            _serverLogBuffer.Add(new ServerLogEntry
                            {
                                Message = stderr.Text,
                                Severity = ServerLogSeverity.Error,
                                Timestamp = DateTime.UtcNow,
                            });
                            Console.Error.WriteLine($"[Server ERROR] {stderr.Text}");
                            break;

                        case StartedCommandEvent started:
                            _serverLogBuffer.Add(new ServerLogEntry
                            {
                                Message = "Server started",
                                Severity = ServerLogSeverity.Info,
                                Timestamp = DateTime.UtcNow,
                            });
                            Console.WriteLine($"Server started (PID: {started.ProcessId})");
                            _serverStatus = ServerStatus.Running;
                            _runningSince = DateTime.UtcNow;
                            break;

                        case ExitedCommandEvent exited:
                            _serverLogBuffer.Add(new ServerLogEntry
                            {
                                Message = "Server Stopped",
                                Severity = ServerLogSeverity.Info,
                                Timestamp = DateTime.UtcNow,
                            });
                            Console.WriteLine($"Server exited with code {exited.ExitCode}");
                            _runningSince = null;
                            _serverStatus = ServerStatus.Stopped;
                            break;
                    }
                }
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Server cancellation requested.");
            }
            catch (Exception ex)
            {
                _serverStatus = ServerStatus.Stopped;
                _runningSince = null;
                Console.Error.WriteLine($"Server crashed: {ex.Message}");
            }
        });
    }

    /// <summary>
    /// Stop the Arma 3 server
    /// </summary>
    public async Task StopServer()
    {
        if (_updateTask is not null)
            return;

        if (_serverTask == null || _serverTask.IsCompleted)
        {
            Console.WriteLine("Server is not running.");
            return;
        }

        Console.WriteLine("Stopping Arma 3 Server...");

        try
        {
            _serverCts?.CancelAsync(); // cancels streaming + sends SIGKILL to child
            await _serverTask; // wait for cleanup
            _serverStatus = ServerStatus.Stopped;
            _runningSince = null;
            Console.WriteLine("Server stopped.");
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error stopping server: {ex.Message}");
        }
        finally
        {
            _serverTask = null;
            _serverCts = null;
        }
    }

    /// <summary>
    /// Get server status and info
    /// </summary>
    public Task<ServerInfo> GetServerInfo()
    {
        var info = new ServerInfo
        {
            Status = _serverStatus,
            ServerPath = _serverDir,
            CurrentPlayers = 0, // Optional: add RPT parsing later
            MaxPlayers = 64,
            RunningSince = _runningSince
        };

        return Task.FromResult(info);
    }

    /// <summary>
    /// Update server (same as download for SteamCMD)
    /// </summary>
    public Guid Update()
    {
        if (_updateTask != null)
        {
            return _updateTask.Id;
        }

        var operationId = Guid.NewGuid();
        _updatesQueue.RegisterUpdater(operationId, out var writer);
        writer.TryWrite(new ServerLogEntry
        {
            Message = "Update started",
            Severity = ServerLogSeverity.Info,
            Timestamp = DateTime.UtcNow,
        });
        var cts = new CancellationTokenSource();
        _updateTask =
            new UpdateOperation(operationId, Task.Run(() => UpdateInternal(writer, cts.Token), cts.Token), cts);
        return operationId;
    }

    public async Task CancelUpdate()
    {
        if (_updateTask == null) return;
        await _updateTask.CancellationTokenSource.CancelAsync();
        await _updateTask.Operation;
        _updatesQueue.ClearUpdates(_updateTask.Id);
        _updateTask = null;
    }

    public ChannelReader<ServerLogEntry>? GetUpdatesReader(Guid updateId)
    {
        return _updatesQueue.GetUpdates(updateId);
    }

    public IEnumerable<ServerLogEntry> GetServerLogs()
    {
        return _serverLogBuffer.Get();
    }

    private async Task UpdateInternal(ChannelWriter<ServerLogEntry> writer, CancellationToken token)
    {
        _serverStatus = ServerStatus.Updating;

        var steamCmdString = new SteamCmdBuilder()
            .WithCredentials(_settings)
            .WithInstallDirectory(_serverDir)
            .WithAppUpdate(ArmA3Constants.ArmA3ServerId)
            .WithQuit()
            .Build();
        Console.WriteLine($"Updating Server... with cmd {steamCmdString}");
        var cmd = Cli.Wrap(_steamCmdPath)
            .WithArguments(steamCmdString)
            .WithValidation(CommandResultValidation.ZeroExitCode);

        await foreach (var ev in cmd.ListenAsync(token))
        {
            switch (ev)
            {
                case StandardOutputCommandEvent stdout:
                    await writer.WriteAsync(new ServerLogEntry
                    {
                        Message = AnsiRegex.Replace(stdout.Text, ""),
                        Severity = ServerLogSeverity.Info,
                        Timestamp = DateTime.UtcNow,
                    }, token);
                    Console.WriteLine($"[ServerUpdate] {stdout.Text}");
                    break;

                case StandardErrorCommandEvent stderr:
                    await writer.WriteAsync(new ServerLogEntry
                    {
                        Message = AnsiRegex.Replace(stderr.Text, ""),
                        Severity = ServerLogSeverity.Error,
                        Timestamp = DateTime.UtcNow,
                    }, token);
                    Console.Error.WriteLine($"[ServerUpdate ERROR] {stderr.Text}");
                    break;
            }
        }

        Console.WriteLine("[ServerUpdate] Update complete");
        writer.Complete();
        _serverStatus = ServerStatus.Initialized;
        if (_updateTask != null)
        {
            _updatesQueue.ClearUpdates(_updateTask.Id);
            _updateTask = null;
        }
    }

    public string Name => "ServerManager";

    public async Task Initialize()
    {
        var op = Update();
        var reader = GetUpdatesReader(op);
        if (reader is null)
        {
            return;
        }

        await foreach (var _ in reader.ReadAllAsync())
        {
            // A naive mind would think one could just await reader.Completion.
            // What a fool, of course Completion only fires when the writer completes the channel
            // **AND** all the data from the Channel has been read. Thus read and do nothing with it
            // ¯\_(ツ)_/¯
        }
    }

    public Task OnInitializationCompleted()
    {
        if (_autoStartServer)
        {
            StartServer();
        }

        return Task.CompletedTask;
    }

    private async Task WriteActiveConfig(ConfigurationBundle configBundle, CancellationToken ct = default)
    {
        var wasRunning = _serverStatus == ServerStatus.Running;
        if (wasRunning)
        {
            await StopServer();
        }

        await using (var file = File.Create(_configFilePath))
        {
            await file.WriteAsync(Encoding.UTF8.GetBytes(configBundle.ServerConfig), ct);
        }

        if (wasRunning)
        {
            StartServer();
        }
    }

    [GeneratedRegex(@"\x1B\[[0-9;]*[A-Za-z]", RegexOptions.Compiled)]
    private static partial Regex GeneratedAnsiRegex();
}
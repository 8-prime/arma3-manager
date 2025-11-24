using System.Text.RegularExpressions;
using System.Threading.Channels;
using ArmA3Manager.Application.Common;
using ArmA3Manager.Application.Common.Enums;
using ArmA3Manager.Application.Common.Interfaces;
using ArmA3Manager.Application.Common.Models;
using ArmA3Manager.Application.Common.Models.Server;
using CliWrap;
using CliWrap.EventStream;
using Microsoft.Extensions.Options;

namespace ArmA3Manager.Application.Services;

public class ServerManager : IServerManager
{
    private static readonly Regex AnsiRegex = new(@"\x1B\[[0-9;]*[A-Za-z]", RegexOptions.Compiled);
    private readonly IUpdatesQueue<string> _updatesQueue;
    private UpdateOperation? _updateTask;
    private readonly string _steamCmdPath;
    private readonly string _armaServerPath;
    private readonly string _serverDir;
    private readonly string _username;
    private readonly string _password;

    private Task? _serverTask;
    private CancellationTokenSource? _serverCts;

    public ServerManager(IOptions<ManagerSettings> managerSettings, IUpdatesQueue<string> updatesQueue)
    {
        _updatesQueue = updatesQueue;
        _steamCmdPath = managerSettings.Value.SteamCmdPath;
        _armaServerPath = managerSettings.Value.ArmaServerPath;
        _serverDir = managerSettings.Value.ServerDir;
        _username = managerSettings.Value.SteamUsername;
        _password = managerSettings.Value.SteamPassword;
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
            .WithArguments("-config=server.cfg")
            .WithWorkingDirectory(_serverDir);

        // Run server in background, streaming logs
        _serverTask = Task.Run(async () =>
        {
            try
            {
                await foreach (var ev in cmd.ListenAsync(_serverCts.Token))
                {
                    switch (ev)
                    {
                        case StandardOutputCommandEvent stdout:
                            Console.WriteLine($"[Server] {stdout.Text}");
                            break;

                        case StandardErrorCommandEvent stderr:
                            Console.Error.WriteLine($"[Server ERROR] {stderr.Text}");
                            break;

                        case StartedCommandEvent started:
                            Console.WriteLine($"Server started (PID: {started.ProcessId})");
                            break;

                        case ExitedCommandEvent exited:
                            Console.WriteLine($"Server exited with code {exited.ExitCode}");
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
            Status = _serverTask is { IsCompleted: false } ? ServerStatus.Running : ServerStatus.Stopped,
            ServerPath = _serverDir,
            CurrentPlayers = 0, // Optional: add RPT parsing later
            MaxPlayers = 64
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
        writer.TryWrite("Update started");
        var cts = new CancellationTokenSource();
        _updateTask =
            new UpdateOperation(operationId, Task.Run(() => UpdateInternal(writer, cts.Token), cts.Token), cts);
        return operationId;
    }

    public async Task CancelUpdate()
    {
        if (_updateTask == null) return;
        await _updateTask.CancellationTokenSource.CancelAsync();
        _updatesQueue.ClearUpdates(_updateTask.Id);
        _updateTask = null;
    }

    public ChannelReader<string>? GetUpdatesReader(Guid updateId)
    {
        return _updatesQueue.GetUpdates(updateId);
    }

    private async Task UpdateInternal(ChannelWriter<string> writer, CancellationToken token)
    {
        var credentials = string.IsNullOrEmpty(_username) || string.IsNullOrEmpty(_password)
            ? "anonymous"
            : $"{_username} {_password}";
        var cmd = Cli.Wrap(_steamCmdPath)
            .WithArguments(
                $"+force_install_dir \"{_serverDir}\" +login {credentials} +app_update {ArmA3Constants.ArmA3ServerId} validate +quit")
            .WithValidation(CommandResultValidation.ZeroExitCode);

        await foreach (var evt in cmd.ListenAsync(token))
        {
            if (evt is StandardOutputCommandEvent stdOut)
            {
                await writer.WriteAsync(AnsiRegex.Replace(stdOut.Text, ""), token);
            }
        }

        writer.Complete();
        if (_updateTask != null)
        {
            _updatesQueue.ClearUpdates(_updateTask.Id);
            _updateTask = null;
        }
    }

    public Task Initialize()
    {
        return Task.CompletedTask;
    }
}
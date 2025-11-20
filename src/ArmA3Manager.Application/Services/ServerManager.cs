using ArmA3Manager.Application.Common;
using ArmA3Manager.Application.Common.Enums;
using ArmA3Manager.Application.Common.Interfaces;
using ArmA3Manager.Application.Common.Models;
using CliWrap;
using CliWrap.Buffered;
using Microsoft.Extensions.Options;

namespace ArmA3Manager.Application.Services;

public class ServerManager : IServerManager
{
    private readonly string _steamCmdPath;
    private readonly string _armaServerPath;
    private readonly string _serverDir;
    private readonly string _username;
    private readonly string _password;

    private System.Diagnostics.Process? _armaProcess;

    public ServerManager(IOptions<ManagerSettings> managerSettings)
    {
        _steamCmdPath = managerSettings.Value.SteamCmdPath;
        _armaServerPath = managerSettings.Value.ArmaServerPath;
        _serverDir = managerSettings.Value.ServerDir;
        _username = managerSettings.Value.SteamUsername;
        _password = managerSettings.Value.SteamPassword;
    }

    public async Task StartServer()
    {
        if (_armaProcess != null && !_armaProcess.HasExited)
        {
            Console.WriteLine("Server is already running.");
            return;
        }

        Console.WriteLine("Starting Arma 3 Server...");

        _armaProcess = new System.Diagnostics.Process
        {
            StartInfo = new System.Diagnostics.ProcessStartInfo
            {
                FileName = _armaServerPath,
                Arguments = "-config=server.cfg",
                WorkingDirectory = _serverDir,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            },
            EnableRaisingEvents = true
        };

        _armaProcess.OutputDataReceived += (sender, args) =>
        {
            if (!string.IsNullOrEmpty(args.Data))
                Console.WriteLine($"[Server] {args.Data}");
        };

        _armaProcess.ErrorDataReceived += (sender, args) =>
        {
            if (!string.IsNullOrEmpty(args.Data))
                Console.Error.WriteLine($"[Server ERROR] {args.Data}");
        };

        _armaProcess.Start();
        _armaProcess.BeginOutputReadLine();
        _armaProcess.BeginErrorReadLine();

        await Task.Delay(500); // small delay to ensure process starts
    }

    /// <summary>
    /// Stop the Arma 3 server
    /// </summary>
    public async Task StopServer()
    {
        if (_armaProcess == null || _armaProcess.HasExited)
        {
            Console.WriteLine("Server is not running.");
            return;
        }

        Console.WriteLine("Stopping Arma 3 Server...");

        try
        {
            _armaProcess.Kill();
            await _armaProcess.WaitForExitAsync();
            Console.WriteLine("Server stopped.");
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error stopping server: {ex.Message}");
        }
        finally
        {
            _armaProcess = null;
        }
    }

    /// <summary>
    /// Get server status and info
    /// </summary>
    public Task<ServerInfo> GetServerInfo()
    {
        var info = new ServerInfo
        {
            Status = _armaProcess is { HasExited: false } ? ServerStatus.Running : ServerStatus.Stopped,
            ServerPath = _serverDir,
            CurrentPlayers = 0, // Optional: add RPT parsing later
            MaxPlayers = 64
        };

        return Task.FromResult(info);
    }

    /// <summary>
    /// Update server (same as download for SteamCMD)
    /// </summary>
    public async Task Update()
    {
        Console.WriteLine("Downloading or updating Arma 3 Dedicated Server...");
        var credentials = string.IsNullOrEmpty(_username) || string.IsNullOrEmpty(_password) ? "anonymous" : $"{_username} {_password}";
        var result = await Cli.Wrap(_steamCmdPath)
            .WithArguments($"+login {credentials} +force_install_dir \"{_serverDir}\" +app_update {ArmA3Constants.ArmA3ServerId} validate +quit")
            .WithValidation(CommandResultValidation.ZeroExitCode)
            .ExecuteBufferedAsync();
        Console.WriteLine(result.StandardOutput);
    }
}
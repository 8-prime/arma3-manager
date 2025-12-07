namespace ArmA3Manager.Application.Common.Models;

public class ManagerSettings
{
    public required string SteamCmdPath { get; set; } = "/steamcmd/steamcmd.sh";
    public string ArmaServerPath => Path.Join(ServerDir, "arma3server_x64");
    public required string ServerDir { get; set; } = "/arma3/server";
    public required string SteamUsername { get; set; }
    public required string SteamPassword { get; set; }
    public string ConfigPath => Path.Join(ServerDir, "server.cfg");
    public string ConfigInfoPath => Path.Join(ServerDir, "configs.json");
    public required string ConfigurationsDir { get; set; } = "/arma3/config";
    public required string MissionsDownloadDir { get; set; }
    public string MissionsDir => Path.Combine(ServerDir, "mpmissions");
    public bool AutoStartServer { get; set; } = true;
    public bool SkipInitialization { get; set; } = false;
}
namespace ArmA3Manager.Application.Common.Models;

public class ManagerSettings
{
    public required string SteamCmdPath { get; set; } = "/steamcmd/steamcmd.sh";
    public string ArmaServerPath => Path.Join(ServerDir, "arma3server_x64");
    public required string ServerDir { get; set; }

    public required string SteamUsername { get; set; }
    public required string SteamPassword { get; set; }
    public string ConfigPath => Path.Join(ConfigDir, "server.cfg");
    public required string ConfigDir { get; set; }
    public string MissionsDir => Path.Combine(ServerDir, "mpmissions");
}
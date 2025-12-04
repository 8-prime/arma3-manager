namespace ArmA3Manager.Application.Common.Models;

public class ManagerSettings
{
    public const string SteamCmdPath = "/steamcmd/steamcmd.sh";
    public static string ArmaServerPath => Path.Join(ServerDir, "arma3server_x64");
    public const string ServerDir = "/arma3/server";
    public required string SteamUsername { get; set; }
    public required string SteamPassword { get; set; }
    public static string ConfigPath => Path.Join(ServerDir, "server.cfg");
    public static string ConfigInfoPath =>  Path.Join(ServerDir, "configs.json");
    public const string ConfigurationsDir = "/arma3/config";
    public required string MissionsDownloadDir { get; set; }
    public static string MissionsDir => Path.Combine(ServerDir, "mpmissions");
    public bool AutoStartServer { get; set; } = true;
}
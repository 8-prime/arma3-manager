namespace ArmA3Manager.Application.Common.Models;

public class ManagerSettings
{
    public const string Position = "ManagerSettings";

    public required string SteamCmdPath { get; set; } = "/steamcmd/steamcmd.sh";
    public required string ArmaServerPath { get; set; }
    public required string ServerDir { get; set; }

    public required string SteamUsername { get; set; }
    public required string SteamPassword { get; set; }
}
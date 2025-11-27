using ArmA3Manager.Application.Common.Enums;

namespace ArmA3Manager.Application.Common.Models;

public class ServerInfo
{
    public int CurrentPlayers { get; set; }
    public int MaxPlayers { get; set; }
    public ServerStatus Status { get; set; } = ServerStatus.NotInitialized;
    public DateTime? RunningSince { get; set; }
    public required string ServerPath { get; set; }
}
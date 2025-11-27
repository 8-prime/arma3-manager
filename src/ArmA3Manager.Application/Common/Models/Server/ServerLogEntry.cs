using ArmA3Manager.Application.Common.Enums;

namespace ArmA3Manager.Application.Common.Models.Server;

public class ServerLogEntry
{
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public ServerLogSeverity Severity { get; set; } = ServerLogSeverity.Info;
    public required string Message { get; set; }
}
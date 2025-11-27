using ArmA3Manager.Application.Common.Enums;

namespace ArmA3Manager.Application.Common.Models.Server;

public class ServerLogEntry
{
    public DateTime Timestamp { get; set; }
    public ServerLogSeverity Severity { get; set; }
    public string Message { get; set; }
}
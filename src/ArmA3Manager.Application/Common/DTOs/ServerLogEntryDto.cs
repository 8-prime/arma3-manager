using ArmA3Manager.Application.Common.Enums;

namespace ArmA3Manager.Application.Common.DTOs;

public class ServerLogEntryDto
{
    public DateTime Timestamp { get; set; }
    public ServerLogSeverity Severity { get; set; }
    public required string Message { get; set; }
}
using ArmA3Manager.Application.Common.Enums;

namespace ArmA3Manager.Web.Common.DTOs;

public class ServerLogEntryDTO
{
    public DateTime Timestamp { get; set; }
    public ServerLogSeverity Severity { get; set; }
    public string Message { get; set; }
}
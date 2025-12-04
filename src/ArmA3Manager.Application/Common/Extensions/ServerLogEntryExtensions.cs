using ArmA3Manager.Application.Common.DTOs;
using ArmA3Manager.Application.Common.Models.Server;

namespace ArmA3Manager.Application.Common.Extensions;

public static class ServerLogEntryExtensions
{
    public static ServerLogEntryDto Map(this ServerLogEntry serverLogEntry)
    {
        return new ServerLogEntryDto
        {
            Message = serverLogEntry.Message,
            Severity = serverLogEntry.Severity,
            Timestamp = serverLogEntry.Timestamp
        };
    }
}
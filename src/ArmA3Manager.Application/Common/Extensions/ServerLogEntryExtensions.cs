using ArmA3Manager.Application.Common.Models.Server;
using ArmA3Manager.Web.Common.DTOs;

namespace ArmA3Manager.Application.Common.Extensions;

public static class ServerLogEntryExtensions
{
    public static ServerLogEntryDTO Map(this ServerLogEntry serverLogEntry)
    {
        return new ServerLogEntryDTO
        {
            Message = serverLogEntry.Message,
            Severity = serverLogEntry.Severity,
            Timestamp = serverLogEntry.Timestamp
        };
    }
}
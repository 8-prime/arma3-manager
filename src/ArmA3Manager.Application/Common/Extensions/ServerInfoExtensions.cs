using ArmA3Manager.Application.Common.DTOs;
using ArmA3Manager.Application.Common.Models;

namespace ArmA3Manager.Application.Common.Extensions;

public static class ServerInfoExtensions
{
    public static ServerInfoDto Map(this ServerInfo info)
    {
        return new ServerInfoDto
        {
            ServerPath = info.ServerPath,
            CurrentPlayers = info.CurrentPlayers,
            MaxPlayers = info.MaxPlayers,
            Status = info.Status,
            RunningSince = info.RunningSince,
        };
    }
}
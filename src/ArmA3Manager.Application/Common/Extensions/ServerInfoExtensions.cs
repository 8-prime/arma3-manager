using ArmA3Manager.Application.Common.Models;
using ArmA3Manager.Web.Common.DTOs;

namespace ArmA3Manager.Application.Common.Extensions;

public static class ServerInfoExtensions
{
    public static ServerInfoDTO Map(this ServerInfo info)
    {
        return new ServerInfoDTO
        {
            ServerPath = info.ServerPath,
            CurrentPlayers = info.CurrentPlayers,
            MaxPlayers = info.MaxPlayers,
            Status = info.Status,
        };
    }
}
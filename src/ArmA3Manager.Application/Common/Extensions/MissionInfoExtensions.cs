using ArmA3Manager.Application.Common.DTOs;
using ArmA3Manager.Application.Common.Models;

namespace ArmA3Manager.Application.Common.Extensions;

public static class MissionInfoExtensions
{
    public static MissionInfoDto Map(this MissionInfo information)
    {
        return new MissionInfoDto
        {
            MissionId = information.MissionId,
            MissionName = information.MissionName,
        };
    }
}
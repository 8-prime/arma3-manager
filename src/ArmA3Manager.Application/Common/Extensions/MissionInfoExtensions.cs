using ArmA3Manager.Application.Common.DTOs;
using ArmA3Manager.Application.Common.Models;

namespace ArmA3Manager.Application.Common.Extensions;

public static class MissionInfoExtensions
{
    public static MissionInfoDto Map(this MissionInfo info)
    {
        return new MissionInfoDto
        {
            MissionId = info.MissionId,
            MissionName = info.MissionName,
        };
    }
}
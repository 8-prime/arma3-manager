using ArmA3Manager.Application.Common.DTOs;
using ArmA3Manager.Application.Common.Extensions;
using ArmA3Manager.Application.Common.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace ArmA3Manager.Web.Endpoints;

public static class MissionsEndpoints
{
    public static WebApplication MapMissionEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("api/missions");
        group.MapGet("", GetMissions);
        group.MapPost("load", LoadMission);
        return app;
    }

    private static async Task<Results<Ok<MissionInfoDto>, NotFound>> LoadMission([FromBody] LoadMissionRequest request,
        [FromServices] IMissionsManager missionsManager)
    {
        var res = await missionsManager.LoadMission(request.MissionLink);
        if (res is null)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(res.Map());
    }

    private static async Task<Ok<IEnumerable<MissionInfoDto>>> GetMissions(
        [FromServices] IMissionsManager missionsManager)
    {
        return TypedResults.Ok((await missionsManager.GetMissions()).Select(m => m.Map()));
    }
}
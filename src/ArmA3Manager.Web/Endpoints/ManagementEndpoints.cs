using ArmA3Manager.Application.Common.Extensions;
using ArmA3Manager.Application.Common.Interfaces;
using ArmA3Manager.Application.Common.Models;
using ArmA3Manager.Web.Common.DTOs;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace ArmA3Manager.Web.Endpoints;

public static class ManagementEndpoints
{
    public static WebApplication MapManagementEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("management");
        group.MapGet("", GetServerInfoAsync);
        group.MapPost("", UpdateServer);
        return app;
    }

    private static async Task<Ok<ServerInfoDTO>> GetServerInfoAsync([FromServices] IServerManager manager)
    {
        return TypedResults.Ok((await manager.GetServerInfo()).Map());
    }

    private static async Task<Ok> UpdateServer([FromServices] IServerManager manager)
    {
        await manager.Update();
        return TypedResults.Ok();
    }
}
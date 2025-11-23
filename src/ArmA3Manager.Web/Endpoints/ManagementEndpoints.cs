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
        group.MapGet("updates/{id:Guid}", UpdateServer);
        group.MapPost("updates", UpdateServer);
        return app;
    }

    private static async Task<Ok<ServerInfoDTO>> GetServerInfoAsync([FromServices] IServerManager manager)
    {
        return TypedResults.Ok((await manager.GetServerInfo()).Map());
    }

    private static Ok<Guid> UpdateServer([FromServices] IServerManager manager)
    {
        var id = manager.Update();
        return TypedResults.Ok(id);
    }

    private static Results<NotFound, ServerSentEventsResult<string>> GetUpdateUpdates(Guid id,
        [FromServices] IServerManager manager, CancellationToken ct)
    {
        var events = manager.GetUpdatesReader(id);
        if (events is null)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.ServerSentEvents(events.ReadAllAsync(ct));
    }
}
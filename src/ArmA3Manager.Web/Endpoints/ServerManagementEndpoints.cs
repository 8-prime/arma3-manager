using ArmA3Manager.Application.Common.Extensions;
using ArmA3Manager.Application.Common.Interfaces;
using ArmA3Manager.Web.Common.DTOs;
using ArmA3Manager.Web.Extensions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace ArmA3Manager.Web.Endpoints;

public static class ServerManagementEndpoints
{
    public static WebApplication MapServerManagementEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("management");
        group.MapGet("", GetServerInfoAsync);
        group.MapGet("ready", GetUpdateProgress);
        group.MapGet("updates/{id:Guid}", GetUpdateProgress);
        group.MapPost("updates", UpdateServer);
        group.MapPost("updates/cancel", CancelServerUpdate);
        group.MapPost("start", StartServerUpdate);
        group.MapPost("stop", StopServerUpdate);
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

    private static Results<NotFound, ServerSentEventsResult<string>> GetUpdateProgress(Guid id,
        [FromServices] IServerManager manager, CancellationToken ct)
    {
        var events = manager.GetUpdatesReader(id);
        if (events is null)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.ServerSentEvents(events.ReadAllAsync(ct).AsSseStream(ct));
    }

    private static async Task<Ok> CancelServerUpdate([FromServices] IServerManager manager, CancellationToken ct)
    {
        await manager.CancelUpdate();
        return TypedResults.Ok();
    }

    private static Ok StartServerUpdate([FromServices] IServerManager manager)
    {
        manager.StartServer();
        return TypedResults.Ok();
    }

    private static async Task<Ok> StopServerUpdate([FromServices] IServerManager manager)
    {
        await manager.StopServer();
        return TypedResults.Ok();
    }
}
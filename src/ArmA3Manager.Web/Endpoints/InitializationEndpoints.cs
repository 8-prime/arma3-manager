using ArmA3Manager.Application.Common.Interfaces;
using ArmA3Manager.Application.Common.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace ArmA3Manager.Web.Endpoints;

public static class InitializationEndpoints
{
    public static WebApplication MapInitializationEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/initialization");
        group.MapGet("", GetInitializationResources);
        return app;
    }

    private static Ok<List<InitializationResource>> GetInitializationResources([FromServices] IInitializationInfo info)
    {
        return TypedResults.Ok(info.InitializationResources);
    }
}
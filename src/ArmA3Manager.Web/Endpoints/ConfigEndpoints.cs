using ArmA3Manager.Application.Common.Interfaces;
using ArmA3Manager.Web.Common.DTOs;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace ArmA3Manager.Web.Endpoints;

public static class ConfigEndpoints
{
    public static WebApplication MapConfigEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("config");
        group.MapGet("", GetConfig);
        group.MapPost("", SetConfig);
        group.MapPost("reset", ResetConfig);
        return app;
    }


    private static async Task<Ok<ConfigDto>> GetConfig([FromServices] IConfigManager configManager,
        CancellationToken cancellationToken)
    {
        return TypedResults.Ok(new ConfigDto
        {
            ConfigurationString = await configManager.GetConfig(cancellationToken),
        });
    }

    private static async Task<Ok> ResetConfig([FromServices] IConfigManager configManager,
        CancellationToken cancellationToken)
    {
        await configManager.ResetConfig(cancellationToken);
        return TypedResults.Ok();
    }

    private static async Task<Ok> SetConfig([FromServices] IConfigManager configManager, [FromBody] ConfigDto config,
        CancellationToken ct)
    {
        await configManager.SetConfig(config.ConfigurationString, ct);
        return TypedResults.Ok();
    }
}
using ArmA3Manager.Application.Common.DTOs;
using ArmA3Manager.Application.Common.Extensions;
using ArmA3Manager.Application.Common.Interfaces;
using ArmA3Manager.Web.Extensions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace ArmA3Manager.Web.Endpoints;

public static class ConfigEndpoints
{
    public static WebApplication MapConfigEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("api/config").RequireInitialization();
        group.MapGet("", GetConfigs);
        group.MapGet("active", GetActiveConfig);
        group.MapPost("", CreateConfig);
        group.MapPut("", UpdateConfig);
        group.MapDelete("{id:guid}", DeleteConfig);
        group.MapPost("{id:guid}/activate", ActivateConfig);

        return app;
    }

    private static async Task<Ok<IEnumerable<ConfigurationBundleDto>>> GetConfigs(
        [FromServices] IConfigManager configManager)
    {
        var configs = await configManager.GetConfigs();
        return TypedResults.Ok(configs.Select(c => c.Map()));
    }

    private static async Task<Ok<ConfigurationBundleDto>> GetActiveConfig(
        [FromServices] IConfigManager configManager)
    {
        var active = await configManager.GetActiveConfig();
        return TypedResults.Ok(active.Map());
    }

    private static async Task<Created<ConfigurationBundleDto>> CreateConfig(
        [FromBody] ConfigurationBundleDto dto,
        [FromServices] IConfigManager configManager)
    {
        var bundle = dto.Map();
        await configManager.CreateConfig(bundle);
        return TypedResults.Created($"/api/config/{bundle.Id}", bundle.Map());
    }

    private static async Task<Ok> UpdateConfig(
        [FromBody] ConfigurationBundleDto dto,
        [FromServices] IConfigManager configManager)
    {
        await configManager.UpdateConfig(dto.Map());
        return TypedResults.Ok();
    }

    private static async Task<Ok> DeleteConfig(
        Guid id,
        [FromServices] IConfigManager configManager)
    {
        await configManager.DeleteConfig(id);
        return TypedResults.Ok();
    }

    private static async Task<Results<Ok, NotFound>> ActivateConfig(
        Guid id,
        [FromServices] IConfigManager configManager)
    {
        // You may want to verify existence first, but following your example → return Ok always.
        await configManager.ActivateConfig(id);
        return TypedResults.Ok();
    }
}
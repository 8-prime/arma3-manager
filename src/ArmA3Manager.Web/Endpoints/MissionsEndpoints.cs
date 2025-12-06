using ArmA3Manager.Application.Common.Interfaces;
using ArmA3Manager.Web.Extensions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace ArmA3Manager.Web.Endpoints;

public static class MissionsEndpoints
{
    public static WebApplication MapMissionEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("api/missions");
        group.MapPost("", HandleMissionUpload);
        return app;
    }

    private static async Task<Results<Ok, BadRequest<string>>> HandleMissionUpload(IFormFile file,
        [FromServices] IMissionsManager manager, CancellationToken ct)
    {
        if (!file.IsZipArchive())
            return TypedResults.BadRequest("Uploaded file is not a valid ZIP archive");

        await manager.UploadMission(file.OpenReadStream(), ct);
        return TypedResults.Ok();
    }
}
using System.IO.Compression;
using ArmA3Manager.Application.Common.Interfaces;
using ArmA3Manager.Web.Extensions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace ArmA3Manager.Web.Endpoints;

public static class ModEndpoints
{
    public static WebApplication MapModEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("api/mods");
        group.MapPost("", HandleModUpload);
        return app;
    }

    private static async Task<Results<Ok, BadRequest<string>>> HandleModUpload(IFormFile file,
        [FromServices] IModsManager manager, CancellationToken ct)
    {
        if (!file.IsZipArchive())
            return TypedResults.BadRequest("Uploaded file is not a valid ZIP archive");

        await manager.UploadMod(file.OpenReadStream(), ct);
        return TypedResults.Ok();
    }
}
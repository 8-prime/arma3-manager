using System.IO.Compression;
using ArmA3Manager.Application.Common.Interfaces;
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

    private static async Task<Results<Ok, BadRequest<string>>> HandleModUpload(HttpRequest request,
        [FromServices] IModsManager manager, CancellationToken ct)
    {
        if (!request.HasFormContentType)
            return TypedResults.BadRequest("Request must be multipart/form-data");

        var form = await request.ReadFormAsync(ct);
        var file = form.Files.GetFile("file");

        if (file is null)
            return TypedResults.BadRequest("No file uploaded under name 'file'");

        if (!IsZipArchive(file))
            return TypedResults.BadRequest("Uploaded file is not a valid ZIP archive");

        // --- Step 2: inspect ZIP ---
        await manager.UploadMod(file.OpenReadStream(), ct);
        return TypedResults.Ok();
    }

    private static bool IsZipArchive(IFormFile file)
    {
        try
        {
            using var stream = file.OpenReadStream();
            using var archive = new ZipArchive(stream, ZipArchiveMode.Read, true);
            return true;
        }
        catch
        {
            return false;
        }
    }
}
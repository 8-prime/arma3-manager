using System.IO.Compression;

namespace ArmA3Manager.Web.Extensions;

public static class IFormFileExtensions
{
    public static bool IsZipArchive(this IFormFile file)
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
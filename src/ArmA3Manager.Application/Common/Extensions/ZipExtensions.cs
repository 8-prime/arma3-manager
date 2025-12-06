using System.IO.Compression;

namespace ArmA3Manager.Application.Common.Extensions;

public static class ZipExtensions
{
    public static string? GetTopLevelDirectory(this ZipArchiveEntry entry)
    {
        if (string.IsNullOrWhiteSpace(entry.FullName) || !entry.FullName.Contains('/'))
            return null;

        var idx = entry.FullName.IndexOf('/');
        return idx > 0 ? entry.FullName[..(idx + 1)] : null;
    }
}
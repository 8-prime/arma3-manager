using System.IO.Compression;
using ArmA3Manager.Application.Common.Interfaces;
using ArmA3Manager.Application.Common.Models;
using Microsoft.Extensions.Options;

namespace ArmA3Manager.Application.Services;

public class ModsManager : IModsManager
{
    private readonly string _serverDirectory;

    public ModsManager(IOptions<ManagerSettings> options)
    {
        _serverDirectory = ManagerSettings.ServerDir;
    }

    public Task Initialize()
    {
        return Task.CompletedTask;
    }

    public async Task UploadMod(Stream modFileStream, CancellationToken ct = default)
    {
        await using var archive = new ZipArchive(modFileStream, ZipArchiveMode.Read, false);
        var dirPrefix = archive.Entries
            .Select(e => GetTopLevelDirectory(e.FullName))
            .FirstOrDefault(x => x is not null);

        if (dirPrefix is null)
            return;

        if (!Directory.Exists(_serverDirectory))
        {
            Directory.CreateDirectory(_serverDirectory);
        }

        foreach (var entry in archive.Entries)
        {
            if (!entry.FullName.StartsWith(dirPrefix))
                continue;

            var relativePath = entry.FullName[dirPrefix.Length..].TrimStart('/', '\\');

            if (string.IsNullOrEmpty(relativePath))
                continue;

            var destination = Path.Combine(_serverDirectory, relativePath);

            if (entry.FullName.EndsWith('/'))
            {
                Directory.CreateDirectory(destination);
            }
            else
            {
                Directory.CreateDirectory(Path.GetDirectoryName(destination)!);
                var stream = await entry.OpenAsync(ct);
                await stream.CopyToAsync(File.Create(destination), ct);
            }
        }
    }


    private static string? GetTopLevelDirectory(string path)
    {
        if (string.IsNullOrWhiteSpace(path) || !path.Contains('/'))
            return null;

        var idx = path.IndexOf('/');
        return idx > 0 ? path[..(idx + 1)] : null;
    }
}
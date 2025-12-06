using System.IO.Compression;
using ArmA3Manager.Application.Common.Extensions;
using ArmA3Manager.Application.Common.Interfaces;
using ArmA3Manager.Application.Common.Models;
using Microsoft.Extensions.Options;

namespace ArmA3Manager.Application.Services;

public class MissionManager : IMissionsManager
{
    private readonly string _mpMissionsDir;


    public MissionManager(IOptions<ManagerSettings> managerSettings)
    {
        _mpMissionsDir = ManagerSettings.MissionsDir;
        Directory.CreateDirectory(_mpMissionsDir);
    }

    public string Name => "MissionManager";

    public Task Initialize()
    {
        return Task.CompletedTask;
    }

    public async Task UploadMission(Stream missionFileStream, CancellationToken ct = default)
    {
        await using var archive = new ZipArchive(missionFileStream, ZipArchiveMode.Read, false);
        var dirPrefix = archive.Entries
            .Select(e => e.GetTopLevelDirectory())
            .FirstOrDefault(x => x is not null);

        if (dirPrefix is null)
            return;

        if (!Directory.Exists(_mpMissionsDir))
        {
            Directory.CreateDirectory(_mpMissionsDir);
        }

        foreach (var entry in archive.Entries)
        {
            if (!entry.FullName.StartsWith(dirPrefix))
                continue;

            var relativePath = entry.FullName[dirPrefix.Length..].TrimStart('/', '\\');

            if (string.IsNullOrEmpty(relativePath))
                continue;

            var destination = Path.Combine(_mpMissionsDir, relativePath);

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
}
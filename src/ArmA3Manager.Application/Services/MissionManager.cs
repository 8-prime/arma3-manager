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
        _mpMissionsDir = managerSettings.Value.MissionsDir;
    }

    public string Name => "MissionManager";

    public Task Initialize()
    {
        Directory.CreateDirectory(_mpMissionsDir);
        return Task.CompletedTask;
    }

    public async Task UploadMission(Stream missionFileStream, CancellationToken ct = default)
    {
        await using var archive = new ZipArchive(missionFileStream, ZipArchiveMode.Read, false);
        await archive.ExtractToDirectoryAsync(_mpMissionsDir, true, ct);
    }
}
using System.IO.Compression;
using ArmA3Manager.Application.Common.Extensions;
using ArmA3Manager.Application.Common.Interfaces;
using ArmA3Manager.Application.Common.Models;
using Microsoft.Extensions.Options;

namespace ArmA3Manager.Application.Services;

public class ModsManager : IModsManager
{
    private readonly string _serverDirectory;

    public ModsManager(IOptions<ManagerSettings> options)
    {
        _serverDirectory = options.Value.ServerDir;
    }

    public string Name => "ModsManager";

    public Task Initialize()
    {
        return Task.CompletedTask;
    }

    public async Task UploadMod(Stream modFileStream, CancellationToken ct = default)
    {
        await using var archive = new ZipArchive(modFileStream, ZipArchiveMode.Read, false);
        await archive.ExtractToDirectoryAsync(_serverDirectory, ct);
    }
}
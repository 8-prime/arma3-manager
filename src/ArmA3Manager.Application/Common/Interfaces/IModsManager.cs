using System.IO.Compression;
using ArmA3Manager.Application.Common.Models;

namespace ArmA3Manager.Application.Common.Interfaces;

public interface IModsManager : IInitializeable
{
    public Task UploadMod(Stream modFileStream, CancellationToken ct = default);
}
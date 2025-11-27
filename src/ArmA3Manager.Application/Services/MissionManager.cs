using ArmA3Manager.Application.Common;
using ArmA3Manager.Application.Common.Constants;
using ArmA3Manager.Application.Common.Interfaces;
using ArmA3Manager.Application.Common.Models;
using CliWrap;
using CliWrap.Buffered;
using Microsoft.Extensions.Options;

namespace ArmA3Manager.Application.Services;

public class MissionManager : IMissionsManager
{
    private readonly string _mpMissionsDir;
    private readonly string _serverDir;
    private readonly string _steamCmdPath;


    public MissionManager(IOptions<ManagerSettings> managerSettings)
    {
        _steamCmdPath = managerSettings.Value.SteamCmdPath;
        _serverDir = managerSettings.Value.ServerDir;
        _mpMissionsDir = managerSettings.Value.MissionsDir;
        Directory.CreateDirectory(_mpMissionsDir);
    }


    public async Task<MissionInfo?> LoadMission(string missionLink)
    {
        var workshopId = WorkshopHelper.ExtractWorkshopId(missionLink);

        if (workshopId == null)
        {
            return null;
        }
        Console.WriteLine($"Downloading workshop mission {workshopId}...");

        // SteamCMD workshop download
        var result = await Cli.Wrap(_steamCmdPath)
            .WithArguments($"+login anonymous +workshop_download_item {ArmA3Constants.ArmA3Id} {workshopId} +quit")
            .WithValidation(CommandResultValidation.ZeroExitCode)
            .ExecuteBufferedAsync();

        Console.WriteLine(result.StandardOutput);

        // Locate the downloaded mission folder (SteamCMD path: steamapps/workshop/content/107410/<WorkshopId>)
        var workshopPath = Path.Combine(_serverDir, "steamapps", "workshop", "content", ArmA3Constants.ArmA3Id, workshopId);
        if (!Directory.Exists(workshopPath))
        {
            Console.Error.WriteLine($"Workshop folder not found: {workshopPath}");
            return null;
        }

        // Copy mission files to MPMissions
        foreach (var file in Directory.GetFiles(workshopPath, "*.pbo", SearchOption.AllDirectories))
        {
            var dest = Path.Combine(_mpMissionsDir, Path.GetFileName(file));
            File.Copy(file, dest, overwrite: true);
            Console.WriteLine($"Copied mission to {dest}");
        }

        Console.WriteLine("Workshop mission download complete.");
        return null;
    }

    public Task UpdateMissions()
    {
        throw new NotImplementedException();
    }

    public Task UpdateMission(string missionLink)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<MissionInfo>> GetMissions()
    {
        throw new NotImplementedException();
    }

    public Task<MissionInfo> GetMission(string missionLink)
    {
        throw new NotImplementedException();
    }

    public Task DeleteMission(string missionLink)
    {
        throw new NotImplementedException();
    }

    public Task Initialize()
    {
        return Task.CompletedTask;
    }
}
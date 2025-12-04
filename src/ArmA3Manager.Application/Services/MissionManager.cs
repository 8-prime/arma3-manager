using ArmA3Manager.Application.Common;
using ArmA3Manager.Application.Common.Builder;
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
    private readonly string _missionsDownloadDir;
    private readonly ManagerSettings _managerSettings;


    public MissionManager(IOptions<ManagerSettings> managerSettings)
    {
        _steamCmdPath = ManagerSettings.SteamCmdPath;
        _serverDir = ManagerSettings.ServerDir;
        _mpMissionsDir = ManagerSettings.MissionsDir;
        _missionsDownloadDir = managerSettings.Value.MissionsDownloadDir;
        _managerSettings = managerSettings.Value;
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

        var argsBuilder = new SteamCmdBuilder()
            .WithCredentials(_managerSettings)
            .WithInstallDirectory(_missionsDownloadDir)
            .WithWorkshopItemId(ArmA3Constants.ArmA3Id, workshopId)
            .WithQuit();
        var result = await Cli.Wrap(_steamCmdPath)
            .WithArguments(argsBuilder.Build())
            .WithValidation(CommandResultValidation.ZeroExitCode)
            .ExecuteBufferedAsync();

        Console.WriteLine(result.StandardOutput);
        return new MissionInfo
        {
            MissionId =  workshopId,
            MissionName = workshopId
        };
        // // Locate the downloaded mission folder (SteamCMD path: steamapps/workshop/content/107410/<WorkshopId>)
        // var workshopPath = Path.Combine(_serverDir, "steamapps", "workshop", "content", ArmA3Constants.ArmA3Id, workshopId);
        // if (!Directory.Exists(workshopPath))
        // {
        //     Console.Error.WriteLine($"Workshop folder not found: {workshopPath}");
        //     return null;
        // }
        //
        // // Copy mission files to MPMissions
        // foreach (var file in Directory.GetFiles(workshopPath, "*.pbo", SearchOption.AllDirectories))
        // {
        //     var dest = Path.Combine(_mpMissionsDir, Path.GetFileName(file));
        //     File.Copy(file, dest, overwrite: true);
        //     Console.WriteLine($"Copied mission to {dest}");
        // }
        //
        // Console.WriteLine("Workshop mission download complete.");
        // return null;
    }

    public Task UpdateMissions()
    {
        throw new NotImplementedException();
    }

    public Task UpdateMission(string missionId)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<MissionInfo>> GetMissions()
    {
        return Task.FromResult(Enumerable.Empty<MissionInfo>());
    }

    public Task<MissionInfo> GetMission(string missionId)
    {
        throw new NotImplementedException();
    }

    public Task DeleteMission(string missionId)
    {
        throw new NotImplementedException();
    }

    public string Name => "MissionManager";

    public Task Initialize()
    {
        return Task.CompletedTask;
    }
}
using ArmA3Manager.Application.Common.Models;

namespace ArmA3Manager.Application.Common.Interfaces;

public interface IMissionsManager
{
    public Task<MissionInfo?> LoadMission(string missionLink);
    public Task UpdateMissions();
    public Task UpdateMission(string missionLink);
    public Task<IEnumerable<MissionInfo>> GetMissions();
    public Task<MissionInfo> GetMission(string missionLink);
    public Task DeleteMission(string missionLink);
}
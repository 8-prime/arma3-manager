using ArmA3Manager.Application.Common.Models;

namespace ArmA3Manager.Application.Common.Interfaces;

public interface IMissionsManager : IInitializeable
{
    public Task<MissionInfo?> LoadMission(string missionLink);
    public Task UpdateMissions();
    public Task UpdateMission(string missionId);
    public Task<IEnumerable<MissionInfo>> GetMissions();
    public Task<MissionInfo> GetMission(string missionId);
    public Task DeleteMission(string missionId);
}
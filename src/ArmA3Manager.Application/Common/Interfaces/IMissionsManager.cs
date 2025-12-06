namespace ArmA3Manager.Application.Common.Interfaces;

public interface IMissionsManager : IInitializeable
{
    public Task UploadMission(Stream missionFileStream, CancellationToken ct = default);
}
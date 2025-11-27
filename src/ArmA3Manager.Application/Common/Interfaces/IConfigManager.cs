namespace ArmA3Manager.Application.Common.Interfaces;

public interface IConfigManager : IInitializeable
{
    public Task<string> GetConfig(CancellationToken ct = default);
    public Task SetConfig(string config, CancellationToken ct = default);
    public Task ResetConfig(CancellationToken ct = default);
}
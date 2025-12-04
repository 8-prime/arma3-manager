using ArmA3Manager.Application.Common.Models;

namespace ArmA3Manager.Application.Common.Interfaces;

public interface IConfigManager : IInitializeable
{
    public Task CreateConfig(ConfigurationBundle bundle, CancellationToken ct = default);
    public Task UpdateConfig(ConfigurationBundle bundle, CancellationToken ct = default);
    public Task DeleteConfig(Guid id, CancellationToken ct = default);
    public Task ActivateConfig(Guid id, CancellationToken ct = default);
    public Task<IEnumerable<ConfigurationBundle>> GetConfigs(CancellationToken ct = default);
    public Task<ConfigurationBundle> GetActiveConfig(CancellationToken ct = default);
}
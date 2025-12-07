using ArmA3Manager.Application.Common.Enums;
using ArmA3Manager.Application.Common.Interfaces;
using ArmA3Manager.Application.Common.Models;
using Microsoft.Extensions.Options;

namespace ArmA3Manager.Application.Services;

public class InitializationInfo : IInitializationInfo
{
    private readonly Dictionary<string, InitializationResource> _resources = [];
    private readonly bool _skipInitialization;

    public InitializationInfo(IOptions<ManagerSettings> options)
    {
        _skipInitialization = options.Value.SkipInitialization;
    }

    public List<InitializationResource> InitializationResources => _resources.Values.ToList();
    public bool FinishedInitialization => _skipInitialization || _resources.Values.All(r => r.Status >= InitializationStatus.Finished);

    public void SetInitializationResource(string resourceId, InitializationResource resource)
    {
        _resources[resourceId] = resource;
    }

    public void UpdateInitializationResource(string resourceId, InitializationStatus status)
    {
        if (_resources.TryGetValue(resourceId, out var resource))
        {
            resource.Status = status;
        }
    }
}
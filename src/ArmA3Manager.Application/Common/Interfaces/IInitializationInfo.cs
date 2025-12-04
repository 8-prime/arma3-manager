using ArmA3Manager.Application.Common.Enums;
using ArmA3Manager.Application.Common.Models;

namespace ArmA3Manager.Application.Common.Interfaces;

public interface IInitializationInfo
{
    public List<InitializationResource> InitializationResources { get; }
    public bool FinishedInitialization { get; }
    public void SetInitializationResource(string resourceId, InitializationResource resource);
    public void UpdateInitializationResource(string resourceId, InitializationStatus status);
}
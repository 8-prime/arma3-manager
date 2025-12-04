using ArmA3Manager.Application.Common.Enums;

namespace ArmA3Manager.Application.Common.Models;

public class InitializationResource
{
    public required string Name { get; set; }
    public InitializationStatus Status { get; set; }
}
namespace ArmA3Manager.Application.Common.Models;

public class ConfigurationBundle
{
    public required Guid Id { get; set; }
    public required bool IsDefault { get; set; }
    public required string LaunchParameters { get; set; } 
    public required string ServerConfig { get; set; } 
    public required string Name { get; set; }
}
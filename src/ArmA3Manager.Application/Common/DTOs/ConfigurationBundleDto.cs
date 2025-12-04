namespace ArmA3Manager.Application.Common.DTOs;

public class ConfigurationBundleDto
{
    public required Guid Id { get; set; }
    public required bool IsDefault { get; set; }
    public required string Name { get; set; }
    public required string ServerConfig { get; set; }
    public required string LaunchParameters  { get; set; }
}
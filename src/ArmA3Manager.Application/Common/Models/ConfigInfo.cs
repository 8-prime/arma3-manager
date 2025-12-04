using System.Diagnostics.CodeAnalysis;

namespace ArmA3Manager.Application.Common.Models;

public class ConfigInfo
{
    public Guid ActiveConfigId { get; set; }
    public Guid DefaultConfigId { get; set; }
    public required string Name { get; set; }

    public ConfigInfo()
    {
    }

    [SetsRequiredMembers]
    public ConfigInfo(ConfigurationBundle bundle)
    {
        ActiveConfigId = bundle.Id;
        Name = bundle.Name;
        if (bundle.IsDefault)
        {
            DefaultConfigId = bundle.Id;
        }
    }
}
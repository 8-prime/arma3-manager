using System.Diagnostics.CodeAnalysis;

namespace ArmA3Manager.Application.Common.Models;

public class ConfigInfo
{
    public Guid ActiveConfigId { get; set; }
    public Guid DefaultConfigId { get; set; }
    public required string ActiveConfigName { get; set; }

    public ConfigInfo()
    {
    }
    
    [SetsRequiredMembers]
    public ConfigInfo(ConfigurationBundle bundle)
    {
        ActiveConfigId = bundle.Id;
        ActiveConfigName = bundle.Name;
        if (bundle.IsDefault)
        {
            DefaultConfigId = bundle.Id;
        }
    }
}
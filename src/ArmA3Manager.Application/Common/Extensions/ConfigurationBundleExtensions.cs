using ArmA3Manager.Application.Common.DTOs;
using ArmA3Manager.Application.Common.Models;

namespace ArmA3Manager.Application.Common.Extensions;

public static class ConfigurationBundleExtensions
{
    public static ConfigurationBundleDto Map(this ConfigurationBundle bundle)
    {
        return new ConfigurationBundleDto
        {
            Id = bundle.Id,
            Name = bundle.Name,
            ServerConfig = bundle.ServerConfig,
            IsDefault = bundle.IsDefault,
            LaunchParameters = bundle.LaunchParameters,
        };
    }

    public static ConfigurationBundle Map(this ConfigurationBundleDto bundle)
    {
        return new ConfigurationBundle
        {
            Id = bundle.Id,
            Name = bundle.Name,
            ServerConfig = bundle.ServerConfig,
            IsDefault = bundle.IsDefault,
            LaunchParameters = bundle.LaunchParameters,
        };
    }
}
using System.Text;
using ArmA3Manager.Application.Common.Constants;
using ArmA3Manager.Application.Common.Interfaces;
using ArmA3Manager.Application.Common.Models;
using Microsoft.Extensions.Options;
using Microsoft.VisualBasic;

namespace ArmA3Manager.Application.Services;

public class ConfigManager : IConfigManager
{
    private readonly string _configurationDirectory;
    private readonly string _configurationFileName;

    public ConfigManager(IOptions<ManagerSettings> settings)
    {
        _configurationDirectory = settings.Value.ConfigDir;
        _configurationFileName = settings.Value.ConfigPath;
    }

    public async Task Initialize()
    {
        if (!Directory.Exists(_configurationDirectory))
        {
            Directory.CreateDirectory(_configurationDirectory);
        }

        if (!File.Exists(_configurationFileName))
        {
            Console.WriteLine("Creating configuration file");
            await using var file = File.Create(_configurationFileName);
            await file.WriteAsync(Encoding.UTF8.GetBytes(ConfigConstants.DefaultConfig));
        }
    }
}
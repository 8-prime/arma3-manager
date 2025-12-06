using System.Text;
using System.Text.Json;
using ArmA3Manager.Application.Common.Constants;
using ArmA3Manager.Application.Common.Exceptions;
using ArmA3Manager.Application.Common.Interfaces;
using ArmA3Manager.Application.Common.Models;
using Microsoft.Extensions.Options;

namespace ArmA3Manager.Application.Services;

public class ConfigManager : IConfigManager
{
    private readonly string _configurationDirectory;
    private readonly string _configurationFileName;
    private readonly string _configInfoFileName;
    private ConfigInfo? _currentConfigInfo;

    public ConfigManager(IOptions<ManagerSettings> settings)
    {
        _configurationDirectory = ManagerSettings.ConfigurationsDir;
        _configurationFileName = ManagerSettings.ConfigPath;
        _configInfoFileName = ManagerSettings.ConfigInfoPath;
    }

    public string Name => "ConfigManager";

    public async Task Initialize()
    {
        if (!Directory.Exists(_configurationDirectory))
        {
            Directory.CreateDirectory(_configurationDirectory);
        }

        if (!File.Exists(_configInfoFileName))
        {
            Console.WriteLine("Creating configuration file");
            ActiveConfig = new ConfigurationBundle
            {
                Id = Guid.NewGuid(),
                IsDefault = true,
                LaunchParameters = "",
                ServerConfig = ConfigConstants.DefaultConfig,
                Name = "Default",
            };

            await using var configBundleFile =
                File.Create(Path.Join(_configurationDirectory, $"{ActiveConfig.Id}.json"));
            var configBundleJson = JsonSerializer.Serialize(ActiveConfig);
            await configBundleFile.WriteAsync(Encoding.UTF8.GetBytes(configBundleJson));
            await WriteConfigInfoForBundle(ActiveConfig);
            await ActivateConfig(ActiveConfig.Id);
        }
    }

    public ConfigurationBundle? ActiveConfig { get; private set; }

    public async Task CreateConfig(ConfigurationBundle bundle, CancellationToken ct = default)
    {
        await using var configBundleFile =
            File.Create(Path.Join(_configurationDirectory, $"{bundle.Id}.json"));
        var configBundleJson = JsonSerializer.Serialize(bundle);
        await configBundleFile.WriteAsync(Encoding.UTF8.GetBytes(configBundleJson), ct);
    }

    public async Task UpdateConfig(ConfigurationBundle bundle, CancellationToken ct = default)
    {
        var active = await GetActiveConfig(ct);
        var writeToServer = active.Id == bundle.Id;
        await using var configBundleFile =
            File.Create(Path.Join(_configurationDirectory, $"{bundle.Id}.json"));
        var configBundleJson = JsonSerializer.Serialize(bundle);
        await configBundleFile.WriteAsync(Encoding.UTF8.GetBytes(configBundleJson), ct);

        if (writeToServer)
        {
            await WriteConfigToServer(bundle, ct);
        }
    }

    public async Task DeleteConfig(Guid id, CancellationToken ct = default)
    {
        var activeId = await GetActiveGuid(ct);
        if (activeId == id)
        {
            //TODO result type with disallow reason
            return;
        }

        File.Delete(Path.Join(_configurationDirectory, $"{id}.json"));
    }

    public async Task ActivateConfig(Guid id, CancellationToken ct = default)
    {
        var filepath = Path.Join(_configurationDirectory, $"{id}.json");
        if (!File.Exists(filepath))
        {
            //TODO result type with info
            return;
        }

        await using var file = File.OpenRead(filepath);
        ActiveConfig = await JsonSerializer.DeserializeAsync<ConfigurationBundle>(file, cancellationToken: ct);
        if (ActiveConfig is null)
        {
            throw new ConfigurationException($"No config for id {id} found");
        }

        await WriteConfigInfoForBundle(ActiveConfig, ct);
        await WriteConfigToServer(ActiveConfig, ct);
    }

    public async Task<IEnumerable<ConfigurationBundle>> GetConfigs(CancellationToken ct = default)
    {
        List<ConfigurationBundle> configs = [];
        var skipFileName = Path.GetFileNameWithoutExtension(_configInfoFileName);
        foreach (var file in Directory.EnumerateFiles(_configurationDirectory, "*.json", SearchOption.AllDirectories))
        {
            if (Path.GetFileNameWithoutExtension(file) == skipFileName)
            {
                continue;
            }

            var configBundle =
                await JsonSerializer.DeserializeAsync<ConfigurationBundle>(File.OpenRead(file), cancellationToken: ct);
            if (configBundle is null)
            {
                continue;
            }

            configs.Add(configBundle);
        }

        return configs;
    }

    public async Task<ConfigurationBundle> GetActiveConfig(CancellationToken ct = default)
    {
        if (ActiveConfig is not null)
        {
            return ActiveConfig;
        }

        var activeId = await GetActiveGuid(ct);

        await using var file = File.OpenRead(Path.Join(_configurationDirectory, $"{activeId}.json"));
        ActiveConfig = await JsonSerializer.DeserializeAsync<ConfigurationBundle>(file, cancellationToken: ct);
        return ActiveConfig ?? throw new ConfigurationException("No active config found");
    }

    private async Task<Guid> GetActiveGuid(CancellationToken ct = default)
    {
        if (_currentConfigInfo is not null)
        {
            return _currentConfigInfo.ActiveConfigId;
        }

        await using var file = File.OpenRead(_configInfoFileName);
        _currentConfigInfo = await JsonSerializer.DeserializeAsync<ConfigInfo>(file, cancellationToken: ct);
        return _currentConfigInfo?.ActiveConfigId ??
               throw new ConfigurationException("Configuration is broken. Please reset server");
    }

    private async Task WriteConfigInfoForBundle(ConfigurationBundle bundle, CancellationToken ct = default)
    {
        _currentConfigInfo = new ConfigInfo(bundle);
        await using var file = File.Create(_configInfoFileName);
        var json = JsonSerializer.Serialize(_currentConfigInfo);
        await file.WriteAsync(Encoding.UTF8.GetBytes(json), ct);
    }

    private async Task WriteConfigToServer(ConfigurationBundle configBundle, CancellationToken ct = default)
    {
        await using var file = File.Create(_configurationFileName);
        await file.WriteAsync(Encoding.UTF8.GetBytes(configBundle.ServerConfig), ct);
    }
}
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
    private readonly IServerManager _serverManager;
    private ConfigInfo? _currentConfigInfo;


    public ConfigManager(IOptions<ManagerSettings> settings, IServerManager serverManager)
    {
        _serverManager = serverManager;
        _configurationDirectory = settings.Value.ConfigurationsDir;
        _configurationFileName = settings.Value.ConfigPath;
        _configInfoFileName = settings.Value.ConfigInfoPath;
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
            await CreateNewDefaultConfig();
        }
        else
        {
            try
            {
                await GetActiveConfig();
            }
            catch (ConfigurationException)
            {
                await CreateNewDefaultConfig();
            }
        }
    }

    public Task OnInitializationCompleted()
    {
        return Task.CompletedTask;
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
        foreach (var filePath in Directory.EnumerateFiles(_configurationDirectory, "*.json",
                     SearchOption.AllDirectories))
        {
            if (Path.GetFileNameWithoutExtension(filePath) == skipFileName)
            {
                continue;
            }

            await using var configFile = File.OpenRead(filePath);
            var configBundle =
                await JsonSerializer.DeserializeAsync<ConfigurationBundle>(configFile, cancellationToken: ct);
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
        ActiveConfig = await LoadJson<ConfigurationBundle>(Path.Join(_configurationDirectory, $"{activeId}.json"), ct);
        return ActiveConfig ?? throw new ConfigurationException("No active config found");
    }

    private async Task CreateNewDefaultConfig(CancellationToken ct = default)
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

        await WriteJson(ActiveConfig, Path.Join(_configurationDirectory, $"{ActiveConfig.Id}.json"), ct);
        await WriteConfigInfoForBundle(ActiveConfig, ct);
        await ActivateConfig(ActiveConfig.Id, ct);
    }

    private async Task<Guid> GetActiveGuid(CancellationToken ct = default)
    {
        if (_currentConfigInfo is not null)
        {
            return _currentConfigInfo.ActiveConfigId;
        }

        _currentConfigInfo = await LoadJson<ConfigInfo>(_configInfoFileName, ct);
        return _currentConfigInfo?.ActiveConfigId ??
               throw new ConfigurationException("Configuration is broken. Please reset server");
    }

    private async Task WriteConfigInfoForBundle(ConfigurationBundle bundle, CancellationToken ct = default)
    {
        _currentConfigInfo = new ConfigInfo(bundle);
        await WriteJson(_currentConfigInfo, _configInfoFileName, ct);
    }

    private async Task WriteConfigToServer(ConfigurationBundle configBundle, CancellationToken ct = default)
    {
        await _serverManager.StopServer();
        await using var file = File.Create(_configurationFileName);
        await file.WriteAsync(Encoding.UTF8.GetBytes(configBundle.ServerConfig), ct);
        // intentionally don't start server as maybe more settings need to be changed?!
    }

    private static async Task<T?> LoadJson<T>(string filePath, CancellationToken ct = default)
    {
        await using var file = File.OpenRead(filePath);
        var result = await JsonSerializer.DeserializeAsync<T>(file, cancellationToken: ct);
        return result;
    }

    private static async Task WriteJson<T>(T data, string filePath, CancellationToken ct = default)
    {
        await using var file = File.Create(filePath);
        await JsonSerializer.SerializeAsync(file, data, cancellationToken: ct);
    }
}
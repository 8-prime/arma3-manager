using System.Net.NetworkInformation;
using System.Text;
using ArmA3Manager.Application.Common.Models;

namespace ArmA3Manager.Application.Common.Builder;

public class SteamCmdBuilder
{
    private readonly List<CmdEntry> _entries = [];

    private record CmdEntry(string Value, int Priority);

    public string Build()
    {
        var sb = new StringBuilder();
        sb.AppendJoin(' ', _entries.OrderBy(e => e.Priority).Select(e => e.Value));
        return sb.ToString();
    }


    public SteamCmdBuilder WithAnonymous()
    {
        _entries.Add(new CmdEntry("+login anonymous", 1));
        return this;
    }

    public SteamCmdBuilder WithCredentials(string username, string password)
    {
        _entries.Add(new CmdEntry($"+login {username} {password}", 1));
        return this;
    }

    public SteamCmdBuilder WithCredentials(ManagerSettings settings)
    {
        if (string.IsNullOrEmpty(settings.SteamUsername) || string.IsNullOrEmpty(settings.SteamPassword))
        {
            WithAnonymous();
        }
        else
        {
            WithCredentials(settings.SteamUsername, settings.SteamPassword);
        }

        return this;
    }

    public SteamCmdBuilder WithInstallDirectory(string installDir)
    {
        _entries.Add(new CmdEntry($"+force_install_dir \"{installDir}\"", 0));
        return this;
    }

    public SteamCmdBuilder WithWorkshopItemId(string appId, string workshopItemId)
    {
        _entries.Add(new CmdEntry($"+workshop_download_item {appId} {workshopItemId}", 2));
        return this;
    }

    public SteamCmdBuilder WithQuit()
    {
        _entries.Add(new CmdEntry("+quit", 3));
        return this;
    }

    public SteamCmdBuilder WithAppUpdate(string appId, bool validate = true)
    {
        if (validate)
        {
            _entries.Add(new CmdEntry($"+app_update {appId} validate", 2));
            return this;
        }

        _entries.Add(new CmdEntry($"+app_update {appId}", 2));
        return this;
    }
}
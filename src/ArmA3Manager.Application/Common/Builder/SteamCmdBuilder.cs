using System.Security;
using System.Text;
using ArmA3Manager.Application.Common.Models;

namespace ArmA3Manager.Application.Common.Builder;

public class SteamCmdBuilder
{
    private struct CmdBuilderItem
    {
        public required string ItemString { get; set; }
        public int Order { get; set; }
    }

    private readonly List<CmdBuilderItem> _items = [];


    public string Build()
    {
        var sb = new StringBuilder();
        sb.AppendJoin(' ', _items.OrderBy(i => i.Order).Select(i => i.ItemString));
        return sb.ToString();
    }

    public SteamCmdBuilder WithAnonymous()
    {
        _items.Add(new CmdBuilderItem { ItemString = "+login anonymous", Order = 1 });
        return this;
    }

    public SteamCmdBuilder WithCredentials(string username, string password)
    {
        _items.Add(new CmdBuilderItem { ItemString = $"+login {username} {password}", Order = 1 });
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

    public SteamCmdBuilder WithInstallDirectory(string installDirectory)
    {
        _items.Add(new CmdBuilderItem { ItemString = $"+force_install_dir \"{installDirectory}\"", Order = 0 });
        return this;
    }

    public SteamCmdBuilder WithUpdateId(string id, bool validate = true)
    {
        _items.Add(validate
            ? new CmdBuilderItem { ItemString = $"+update_id {id} validate", Order = 2 }
            : new CmdBuilderItem { ItemString = $"+update_id {id}", Order = 2 });
        return this;
    }

    public SteamCmdBuilder WithQuit()
    {
        _items.Add(new CmdBuilderItem { ItemString = $"+quit", Order = 3 });
        return this;
    }

    public SteamCmdBuilder WithWorkshopItemId(string appId, string workshopItemId)
    {
        _items.Add(new CmdBuilderItem { ItemString = $"+workshop_download_item {appId} {workshopItemId}", Order = 2 });
        return this;
    }
}
using ArmA3Manager.Application.Common.Models;

namespace ArmA3Manager.Application.Common.Interfaces;

public interface IModsManager
{
    public Task<ModInfo> LoadMod(string modLink);
    public Task UpdateMods();
    public Task UpdateMod(string modLink);
    public Task<IEnumerable<ModInfo>> GetMods();
    public Task<ModInfo> GetMod(string modLink);
    public Task DeleteMod(string modLink);
}
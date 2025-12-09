using System.Threading.Channels;
using ArmA3Manager.Application.Common.Models;
using ArmA3Manager.Application.Common.Models.Server;

namespace ArmA3Manager.Application.Common.Interfaces;

public interface IServerManager : IInitializeable
{
    public void StartServer();
    public Task StopServer();
    public Task<ServerInfo> GetServerInfo();
    public Task<Guid> Update();
    public Task CancelUpdate();
    public ChannelReader<ServerLogEntry>? GetUpdatesReader(Guid updateId);
    public IEnumerable<ServerLogEntry> GetServerLogs();
}
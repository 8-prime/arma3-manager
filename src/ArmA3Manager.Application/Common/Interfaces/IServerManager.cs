using System.Threading.Channels;
using ArmA3Manager.Application.Common.Models;
using ArmA3Manager.Application.Common.Models.Server;
using Microsoft.Extensions.Hosting;

namespace ArmA3Manager.Application.Common.Interfaces;

public interface IServerManager : IInitializeable
{
    public bool Ready { get; }
    public void StartServer();
    public Task StopServer();
    public Task<ServerInfo> GetServerInfo();
    public Guid Update();
    public Task CancelUpdate();
    public ChannelReader<string>? GetUpdatesReader(Guid updateId);
    public IEnumerable<ServerLogEntry> GetServerLogs();
}
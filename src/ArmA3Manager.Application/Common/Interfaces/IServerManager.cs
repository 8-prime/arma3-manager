using System.Threading.Channels;
using ArmA3Manager.Application.Common.Models;

namespace ArmA3Manager.Application.Common.Interfaces;

public interface IServerManager
{
    public Task StartServer();
    public Task StopServer();
    public Task<ServerInfo> GetServerInfo();
    public Guid Update();
    public ChannelReader<string>? GetUpdatesReader(Guid updateId);
}
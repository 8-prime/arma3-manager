using System.Threading.Channels;
using ArmA3Manager.Application.Common.Models;
using Microsoft.Extensions.Hosting;

namespace ArmA3Manager.Application.Common.Interfaces;

public interface IServerManager: IHostedService
{
    public Task StartServer();
    public Task StopServer();
    public Task<ServerInfo> GetServerInfo();
    public Guid Update();
    public ChannelReader<string>? GetUpdatesReader(Guid updateId);
}
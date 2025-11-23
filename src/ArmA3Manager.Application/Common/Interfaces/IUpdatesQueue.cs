using System.Threading.Channels;

namespace ArmA3Manager.Application.Common.Interfaces;

public interface IUpdatesQueue<T>
{
    public ChannelReader<T>? GetUpdates(Guid updateId);
    public void RegisterUpdater(Guid updateId, out ChannelWriter<T> writer);
}
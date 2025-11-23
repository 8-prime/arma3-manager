using System.Collections.Concurrent;
using System.Threading.Channels;
using ArmA3Manager.Application.Common.Interfaces;

namespace ArmA3Manager.Application.Services;

public class UpdatesQueue<T> : IUpdatesQueue<T>
{
    private readonly ConcurrentDictionary<Guid, Channel<T>> _updates = [];

    public ChannelReader<T>? GetUpdates(Guid updateId)
    {
        if (!_updates.TryGetValue(updateId, out var channel))
        {
            return null;
        }

        //Only one reader can ever exist
        _updates.TryRemove(updateId, out _);
        return channel.Reader;
    }

    public void RegisterUpdater(Guid updateId, out ChannelWriter<T> writer)
    {
        var channel = _updates.GetOrAdd(updateId, _ => Channel.CreateUnbounded<T>());
        writer = channel.Writer;
    }

    public void ClearUpdates(Guid updateId)
    {
        _updates.TryRemove(updateId, out _);
    }
}
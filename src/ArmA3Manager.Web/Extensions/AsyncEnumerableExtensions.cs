using System.Net.ServerSentEvents;
using System.Runtime.CompilerServices;

namespace ArmA3Manager.Web.Extensions;

public static class AsyncEnumerableExtensions
{
    public static async IAsyncEnumerable<SseItem<T>> AsSseStream<T>(
        this IAsyncEnumerable<T> source,
        T finalItem,
        [EnumeratorCancellation] CancellationToken ct = default
    )
    {
        await foreach (var line in source.WithCancellation(ct))
        {
            yield return new SseItem<T>(line, "update");
        }

        yield return new SseItem<T>(finalItem, "done");
    }
}
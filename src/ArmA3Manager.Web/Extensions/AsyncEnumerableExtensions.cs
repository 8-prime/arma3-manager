using System.Net.ServerSentEvents;
using System.Runtime.CompilerServices;

namespace ArmA3Manager.Web.Extensions;

public static class AsyncEnumerableExtensions
{
    public static async IAsyncEnumerable<SseItem<string>> AsSseStream(
        this IAsyncEnumerable<string> source,
        [EnumeratorCancellation] CancellationToken ct = default
    )
    {
        await foreach (var line in source.WithCancellation(ct))
        {
            yield return new SseItem<string>(line, "update");
        }

        yield return new SseItem<string>("finished", "done");
    }
}
<script lang="ts">
    interface SseItem {
        data: string | null;
        eventType: string | null;
        eventId: string | null;
        reconnectionInterval: string | null;
    }

    let logs: { text: string; type: string }[] = [];
    let eventSource: EventSource | null = null;
    let isRunning = false;
    let logContainer: HTMLDivElement;

    function appendLog(entry: { text: string; type: string }) {
        logs = [...logs, entry].slice(-50);

        const shouldScroll =
            logContainer.scrollTop + logContainer.clientHeight >=
            logContainer.scrollHeight - 5;

        requestAnimationFrame(() => {
            if (shouldScroll) {
                logContainer.scrollTop = logContainer.scrollHeight;
            }
        });
    }

    async function startUpdate() {
        isRunning = true;
        logs = [];

        const res = await fetch("/management/updates", { method: "POST" });
        if (!res.ok) {
            appendLog({ text: "Failed to initiate update.", type: "error" });
            return;
        }

        const id = await res.json();
        eventSource = new EventSource(`/management/updates/${id}`);

        eventSource.addEventListener("update", (e) => {
            appendLog({ text: e.data, type: "info" });
        });

        eventSource.addEventListener("done", (e) => {
            appendLog({ text: "✅ Update completed", type: "info" });
            eventSource?.close();
            isRunning = false;
        });

        eventSource.addEventListener("error", () => {
            appendLog({ text: "⚠ Stream disconnected", type: "warning" });
            eventSource?.close();
            isRunning = false;
        });
    }

    async function stopStream() {
        const res = await fetch("/management/updates/cancel", {
            method: "POST",
        });
        if (!res.ok) {
            appendLog({ text: "Failed to cancel update.", type: "error" });
            return;
        }
        appendLog({ text: "⛔ Stream stopped manually", type: "warning" });
        isRunning = false;
    }
</script>

<!-- Controls -->
<div class="flex gap-2 mb-4">
    <button
        class="px-4 py-2 bg-blue-600 text-white rounded hover:bg-blue-700 disabled:opacity-40"
        on:click={startUpdate}
        disabled={isRunning}
    >
        Start Update
    </button>

    <button
        class="px-4 py-2 bg-red-600 text-white rounded hover:bg-red-700 disabled:opacity-40"
        on:click={stopStream}
        disabled={!isRunning}
    >
        Stop
    </button>
</div>

<!-- Log window -->
<div
    bind:this={logContainer}
    class="bg-black text-green-400 font-mono p-4 rounded border border-neutral-700 h-56 overflow-y-auto whitespace-pre-wrap"
>
    {#each logs.slice(-10) as log}
        <div
            class={"py-0.5 " +
                (log.type === "info"
                    ? "text-green-400"
                    : log.type === "warning"
                      ? "text-yellow-400"
                      : "text-red-400")}
        >
            {log.text}
        </div>
    {/each}
</div>

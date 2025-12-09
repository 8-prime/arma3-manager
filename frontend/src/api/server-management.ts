import type { ServerInfoDTO, ServerLogEntryDTO } from "./types";

export const getServerInfo = async (): Promise<ServerInfoDTO> => {
    const res = await fetch(`api/management`);
    if (!res.ok) throw new Error("Failed to get server info");
    return res.json();
};

export const getServerLogs = async (): Promise<ServerLogEntryDTO[]> => {
    const res = await fetch(`api/management/logs`);
    if (!res.ok) throw new Error("Failed to fetch server logs");
    return res.json();
};

export const startServer = async (): Promise<void> => {
    const res = await fetch(`api/management/start`, {
        method: "POST",
    });
    if (!res.ok) throw new Error("Failed to start server");
};

export const stopServer = async (): Promise<void> => {
    const res = await fetch(`api/management/stop`, {
        method: "POST",
    });
    if (!res.ok) throw new Error("Failed to stop server");
};

// ---- /management/updates ----
export const createUpdate = async (): Promise<string> => {
    const res = await fetch(`api/management/updates`, {
        method: "POST",
    });
    if (!res.ok) throw new Error("Failed to create update");
    return res.json();
};

export const cancelUpdate = async (): Promise<void> => {
    const res = await fetch(`api/management/updates/cancel`, {
        method: "POST",
    });
    if (!res.ok) throw new Error("Failed to cancel update");
};

// ---- /management/updates/{id} (SSE) ----
export const subscribeUpdateStream = (
    id: string,
    onMessage: (item: ServerLogEntryDTO) => void,
    onComplete: () => void
): EventSource => {
    const eventSource = new EventSource(`api/management/updates/${id}`);

    eventSource.addEventListener("update", (e) => {
        console.log(e.data);
        onMessage(e.data);
    });

    eventSource.addEventListener("done", (e) => {
        onMessage(e.data);
        onComplete();
        eventSource?.close();
    });

    eventSource.addEventListener("error", () => {
        onMessage({
            message: "⚠️ Stream disconnected",
            severity: "Error",
            timestamp: new Date().toString()
        })
        onComplete();
        eventSource?.close();
    });

    return eventSource;
};
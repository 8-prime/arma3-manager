import type { InitializationResource } from "./types";

export const getInitializationStatus = async (): Promise<InitializationResource[]> => {
    const res = await fetch(`api/initialization`);
    if (!res.ok) throw new Error("Failed to get server info");
    return res.json();
};
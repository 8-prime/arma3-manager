import type { ConfigDto } from "./types";

const jsonHeaders = { "Content-Type": "application/json" };

export const getConfig = async (
): Promise<ConfigDto> => {
    const res = await fetch(`api/config`);
    if (!res.ok) throw new Error("Failed to load config");
    return res.json();
};

export const setConfig = async (
    body: ConfigDto,
): Promise<void> => {
    const res = await fetch(`api/config`, {
        method: "POST",
        headers: jsonHeaders,
        body: JSON.stringify(body),
    });
    if (!res.ok) throw new Error("Failed to set config");
};

export const resetConfig = async (
): Promise<void> => {
    const res = await fetch(`api/config/reset`, {
        method: "POST",
    });
    if (!res.ok) throw new Error("Failed to reset config");
};

import type { ConfigurationBundleDto } from "./types";

const BASE_URL = "/api/config";

export async function getConfigs(): Promise<ConfigurationBundleDto[]> {
    const res = await fetch(`${BASE_URL}`);
    if (!res.ok) throw new Error("Failed to get configs");
    return await res.json();
}

export async function getActiveConfig(): Promise<ConfigurationBundleDto> {
    const res = await fetch(`${BASE_URL}/active`);
    if (!res.ok) throw new Error("Failed to get active config");
    return await res.json();
}

export async function createConfig(dto: ConfigurationBundleDto): Promise<ConfigurationBundleDto> {
    const res = await fetch(`${BASE_URL}`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(dto)
    });

    if (!res.ok) throw new Error("Failed to create config");
    return await res.json();
}

export async function updateConfig(dto: ConfigurationBundleDto): Promise<void> {
    const res = await fetch(`${BASE_URL}`, {
        method: "PUT",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(dto)
    });

    if (!res.ok) throw new Error("Failed to update config");
}

export async function deleteConfig(id: string): Promise<void> {
    const res = await fetch(`${BASE_URL}/${id}`, {
        method: "DELETE"
    });

    if (!res.ok) throw new Error("Failed to delete config");
}

export async function activateConfig(id: string): Promise<void> {
    const res = await fetch(`${BASE_URL}/${id}/activate`, {
        method: "POST"
    });

    if (!res.ok) throw new Error("Failed to activate config");
}

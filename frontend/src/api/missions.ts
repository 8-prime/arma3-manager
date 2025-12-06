export async function uploadMission(file: File): Promise<any> {
    if (file.type !== "application/zip" && !file.name.endsWith(".zip")) {
        throw new Error("Selected file is not a ZIP archive");
    }

    const formData = new FormData();
    formData.append("file", file);

    const response = await fetch("/api/missions", {
        method: "POST",
        body: formData,
    });

    if (!response.ok) {
        const message = await response.text();
        throw new Error(`Upload failed: ${message}`);
    }

    return response.json().catch(() => null); // endpoint might return plain text
}

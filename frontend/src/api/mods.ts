export function uploadMod(
    file: File,
    onProgress?: (percent: number) => void
): Promise<any> {
    return new Promise((resolve, reject) => {
        if (file.type !== "application/zip" && !file.name.endsWith(".zip")) {
            reject(new Error("Selected file is not a ZIP archive"));
            return;
        }

        const xhr = new XMLHttpRequest();
        xhr.open("POST", "/api/mods");

        xhr.upload.onprogress = (e) => {
            if (e.lengthComputable && onProgress) {
                const percent = Math.round((e.loaded / e.total) * 100);
                onProgress(percent);
            }
        };

        xhr.onload = () => {
            if (xhr.status >= 200 && xhr.status < 300) {
                try {
                    resolve(JSON.parse(xhr.responseText));
                } catch {
                    resolve(null);
                }
            } else {
                reject(new Error(xhr.responseText));
            }
        };

        xhr.onerror = () => reject(new Error("Upload failed"));

        const formData = new FormData();
        formData.append("file", file);
        xhr.send(formData);
    });
}

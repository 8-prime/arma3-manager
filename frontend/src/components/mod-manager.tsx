import type React from "react"

import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card"
import { Button } from "@/components/ui/button"
import { Input } from "@/components/ui/input"
import { Upload } from "lucide-react"
import { uploadMod } from "@/api/mods"
import { useState } from "react"

export function ModManager() {
    const [uploading, setUploading] = useState<boolean>(false);

    const handleFileUpload = async (event: React.ChangeEvent<HTMLInputElement>) => {
        const files = event.target.files
        if (!files || files.length === 0) return
        setUploading(true);
        await uploadMod(files[0])
        setUploading(false);
    }

    return (
        <Card>
            <CardHeader>
                <CardTitle>Mod Management</CardTitle>
                <CardDescription>Upload and manage server mods</CardDescription>
            </CardHeader>
            <CardContent className="space-y-4">
                <div className="flex items-center gap-4">
                    <Input
                        id="mod-upload"
                        type="file"
                        multiple
                        className="flex-1"
                        onChange={handleFileUpload}
                        accept=".zip,.rar,.7z"
                    />
                    <Button asChild disabled={uploading}>
                        <label htmlFor="mod-upload" className="cursor-pointer">
                            <Upload className="mr-2 h-4 w-4" />
                            Upload Mod
                        </label>
                    </Button>
                </div>
            </CardContent>
        </Card>
    )
}

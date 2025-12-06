import type React from "react"

import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card"
import { Button } from "@/components/ui/button"
import { Input } from "@/components/ui/input"
import { Upload } from "lucide-react"
import { useState } from "react"

export type UploadManagerProps = {
    handleUpload: (file: File) => Promise<any>
    title: string,
    description: string,
    action: string,
    accept: string
}

export function UploadManager({ handleUpload, title, description, action, accept }: Readonly<UploadManagerProps>) {
    const [uploading, setUploading] = useState<boolean>(false);

    const handleFileUpload = async (event: React.ChangeEvent<HTMLInputElement>) => {
        const files = event.target.files
        if (!files || files.length === 0) return
        setUploading(true);
        await handleUpload(files[0])
        setUploading(false);
    }

    return (
        <Card>
            <CardHeader>
                <CardTitle>{title}</CardTitle>
                <CardDescription>{description}</CardDescription>
            </CardHeader>
            <CardContent className="space-y-4">
                <div className="flex items-center gap-4">
                    <Input
                        id="mod-upload"
                        type="file"
                        multiple
                        className="flex-1"
                        onChange={handleFileUpload}
                        accept={accept}
                    />
                    <Button asChild disabled={uploading}>
                        <label htmlFor="mod-upload" className="cursor-pointer">
                            <Upload className="mr-2 h-4 w-4" />
                            {action}
                        </label>
                    </Button>
                </div>
            </CardContent>
        </Card>
    )
}

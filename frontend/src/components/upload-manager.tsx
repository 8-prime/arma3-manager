import { useState } from "react";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "./ui/card";
import { Input } from "./ui/input";
import { Button } from "./ui/button";
import { Upload } from "lucide-react";
import { Progress } from "./ui/progress";

export type UploadManagerProps = {
    handleUpload: (file: File, onProgress?: (percent: number) => void) => Promise<any>;
    title: string;
    description: string;
    action: string;
    accept: string;
};

export function UploadManager({
    handleUpload,
    title,
    description,
    action,
    accept
}: Readonly<UploadManagerProps>) {
    const [uploading, setUploading] = useState(false);
    const [progress, setProgress] = useState<number | null>(null);

    const handleFileUpload = async (event: React.ChangeEvent<HTMLInputElement>) => {
        const files = event.target.files;
        if (!files || files.length === 0) return;

        setUploading(true);
        setProgress(0);

        try {
            await handleUpload(files[0], (p) => setProgress(p));
        } finally {
            setUploading(false);
            setProgress(null);
        }
    };

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
                        disabled={uploading}
                    />
                    <Button asChild disabled={uploading}>
                        <label htmlFor="mod-upload" className="cursor-pointer">
                            <Upload className="mr-2 h-4 w-4" />
                            {uploading ? `Uploading…` : action}
                        </label>
                    </Button>
                </div>

                {progress !== null && (
                    <div className="w-full h-2 bg-muted rounded">
                        <div
                            className="h-2 bg-primary rounded transition-all"
                            style={{ width: `${progress}%` }}
                        />
                        <Progress value={progress} />
                    </div>
                )}
            </CardContent>
        </Card>
    );
}

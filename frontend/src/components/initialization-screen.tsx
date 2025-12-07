import { useEffect, useState } from "react"
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card"
import { CheckCircle2, Circle, Loader2, XCircle } from "lucide-react"
import type { InitializationResource, InitializationStatus } from "@/api/types"
import { getInitializationStatus } from "@/api/initialization"

export type InitializationScreenProps = {
    onComplete: () => void
}

export function InitializationScreen({ onComplete }: Readonly<InitializationScreenProps>) {
    const [resources, setResources] = useState<InitializationResource[]>([])

    useEffect(() => {
        let isMounted = true;

        async function fetchInit() {
            const data = await getInitializationStatus();
            if (data.every(e => e.status == "Finished")) {
                setTimeout(onComplete, 500)
            }
            if (isMounted) setResources(data);
        }

        fetchInit();

        const handle = setInterval(fetchInit, 3000);

        return () => {
            isMounted = false;
            clearInterval(handle);
        };
    }, []);

    const getStatusIcon = (status: InitializationStatus) => {
        switch (status) {
            case "Created":
                return <Circle className="h-5 w-5 text-muted-foreground" />
            case "Started":
                return <Loader2 className="h-5 w-5 animate-spin text-primary" />
            case "Finished":
                return <CheckCircle2 className="h-5 w-5 text-primary" />
            case "Failed":
                return <XCircle className="h-5 w-5 text-destructive" />
        }
    }

    const getStatusText = (status: InitializationStatus) => {
        switch (status) {
            case "Created":
                return "Pending"
            case "Started":
                return "Initializing..."
            case "Finished":
                return "Ready"
            case "Failed":
                return "Failed"
        }
    }

    const allFinished = resources.every((r) => r.status === "Finished")
    const progress = (resources.filter((r) => r.status === "Finished").length / resources.length) * 100

    return (
        <div className="flex min-h-screen items-center justify-center bg-background">
            <Card className="w-full max-w-2xl">
                <CardHeader>
                    <div className="flex items-center gap-3">
                        <div className="flex h-12 w-12 items-center justify-center rounded-lg bg-primary">
                            <svg className="h-7 w-7 text-primary-foreground" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                                <path
                                    strokeLinecap="round"
                                    strokeLinejoin="round"
                                    strokeWidth={2}
                                    d="M9 3v2m6-2v2M9 19v2m6-2v2M5 9H3m2 6H3m18-6h-2m2 6h-2M7 19h10a2 2 0 002-2V7a2 2 0 00-2-2H7a2 2 0 00-2 2v10a2 2 0 002 2zM9 9h6v6H9V9z"
                                />
                            </svg>
                        </div>
                        <div>
                            <CardTitle className="text-2xl">ArmA3 Server Manager</CardTitle>
                            <CardDescription>
                                {allFinished ? "Initialization complete" : "Initializing server management system..."}
                            </CardDescription>
                        </div>
                    </div>
                </CardHeader>
                <CardContent className="space-y-6">
                    {/* Progress bar */}
                    <div className="space-y-2">
                        <div className="flex items-center justify-between text-sm">
                            <span className="text-muted-foreground">Progress</span>
                            <span className="font-mono text-foreground">{Math.round(progress)}%</span>
                        </div>
                        <div className="h-2 w-full overflow-hidden rounded-full bg-secondary">
                            <div
                                className="h-full bg-primary transition-all duration-500 ease-out"
                                style={{ width: `${progress}%` }}
                            />
                        </div>
                    </div>

                    {/* Resource list */}
                    <div className="space-y-3">
                        {resources.map((resource) => (
                            <div
                                key={resource.name}
                                className="flex items-center justify-between rounded-lg border border-border bg-card p-4"
                            >
                                <div className="flex items-center gap-3">
                                    {getStatusIcon(resource.status)}
                                    <span className="font-medium text-foreground">{resource.name}</span>
                                </div>
                                <span className="text-sm text-muted-foreground">{getStatusText(resource.status)}</span>
                            </div>
                        ))}
                    </div>

                    {allFinished && (
                        <div className="rounded-lg border border-primary/20 bg-primary/10 p-4 text-center">
                            <p className="text-sm font-medium text-primary">System ready. Loading interface...</p>
                        </div>
                    )}
                </CardContent>
            </Card>
        </div>
    )
}

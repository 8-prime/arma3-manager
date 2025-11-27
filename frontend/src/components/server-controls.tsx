
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card"
import { Button } from "@/components/ui/button"
import { Play, Square, Download, X } from "lucide-react"
import { Badge } from "@/components/ui/badge"
import { useState } from "react"
import { cancelUpdate, createUpdate, startServer, stopServer, subscribeUpdateStream } from "@/api/server-management"
import { UpdateLog } from "./update-log"
import type { ServerLogEntryDTO } from "@/api/types"

export function ServerControls() {

    const [isUpdating, setIsUpdating] = useState(false)
    const [updateLogs, setUpdateLogs] = useState<ServerLogEntryDTO[]>([])

    const handleUpdate = async () => {
        setIsUpdating(true);
        const updateId = await createUpdate();
        subscribeUpdateStream(updateId, (newEntry) => {
            setUpdateLogs(prev => [...prev, newEntry].slice(-50))
        }, () => setIsUpdating(false))
    }

    const handleCancelUpdate = async () => {
        await cancelUpdate();
    }

    const handleServerStart = async () => {
        await startServer();
    }

    const handleServerStop = async () => {
        await stopServer();
    }

    return (
        <>
            <Card>
                <CardHeader>
                    <div className="flex items-center justify-between">
                        <div>
                            <CardTitle>Server Controls</CardTitle>
                            <CardDescription>Manage your ArmA3 dedicated server</CardDescription>
                        </div>
                    </div>
                </CardHeader>
                <CardContent>
                    <div className="grid gap-4 md:grid-cols-4">
                        <Button
                            size="lg"
                            className="bg-primary text-primary-foreground hover:bg-primary/90"
                            onClick={handleServerStart}
                        >
                            <Play className="mr-2 h-4 w-4" />
                            Start Server
                        </Button>

                        <Button size="lg" variant="destructive" onClick={handleServerStop}>
                            <Square className="mr-2 h-4 w-4" />
                            Stop Server
                        </Button>

                        {isUpdating ? (
                            <Button size="lg" variant="destructive" onClick={handleCancelUpdate}>
                                <X className="mr-2 h-4 w-4" />
                                Cancel Update
                            </Button>
                        ) : (
                            <Button size="lg" variant="outline" onClick={handleUpdate}>
                                <Download className="mr-2 h-4 w-4" />
                                Update Server
                            </Button>
                        )}
                    </div>
                </CardContent>
            </Card>

            {isUpdating && <UpdateLog logs={updateLogs} />}
        </>
    )
}

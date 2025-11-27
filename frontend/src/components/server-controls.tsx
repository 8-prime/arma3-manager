
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card"
import { Button } from "@/components/ui/button"
import { Play, Square, RefreshCw, Download, X } from "lucide-react"
import { Badge } from "@/components/ui/badge"
import { useState, useEffect } from "react"
import { UpdateLog } from "./update-log"

export function ServerControls() {
    const serverVersion = "2.18.151618"
    const isRunning = true

    const [isUpdating, setIsUpdating] = useState(false)
    const [updateLogs, setUpdateLogs] = useState<string[]>([])
    const [abortController, setAbortController] = useState<AbortController | null>(null)

    useEffect(() => {
        return () => {
            if (abortController) {
                abortController.abort()
            }
        }
    }, [abortController])

    const handleUpdate = async () => {
        setIsUpdating(true)
        setUpdateLogs([])

        const controller = new AbortController()
        setAbortController(controller)

        const mockLogs = [
            "Initializing update process...",
            "Checking for updates...",
            "Connecting to Steam servers...",
            "Found update: ArmA3 Server v2.18.151720",
            "Downloading update files... (0%)",
            "Downloading update files... (15%)",
            "Downloading update files... (32%)",
            "Downloading update files... (48%)",
            "Downloading update files... (67%)",
            "Downloading update files... (85%)",
            "Downloading update files... (100%)",
            "Verifying game files...",
            "Installing update...",
            "Updating server configuration...",
            "Update completed successfully!",
        ]

        try {
            for (let i = 0; i < mockLogs.length; i++) {
                if (controller.signal.aborted) {
                    setUpdateLogs((prev) => [...prev, "Update cancelled by user."])
                    break
                }

                await new Promise((resolve) => setTimeout(resolve, 800 + Math.random() * 400))

                if (!controller.signal.aborted) {
                    setUpdateLogs((prev) => [...prev, mockLogs[i]])
                }
            }
        } catch (error) {
            console.log("[v0] Update process error:", error)
        } finally {
            if (!controller.signal.aborted) {
                setTimeout(() => {
                    setIsUpdating(false)
                    setAbortController(null)
                }, 2000)
            } else {
                setIsUpdating(false)
                setAbortController(null)
            }
        }
    }

    const handleCancelUpdate = () => {
        if (abortController) {
            abortController.abort()
        }
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
                        <Badge variant="outline" className="font-mono text-xs">
                            v{serverVersion}
                        </Badge>
                    </div>
                </CardHeader>
                <CardContent>
                    <div className="grid gap-4 md:grid-cols-4">
                        <Button
                            size="lg"
                            disabled={isRunning || isUpdating}
                            className="bg-primary text-primary-foreground hover:bg-primary/90"
                        >
                            <Play className="mr-2 h-4 w-4" />
                            Start Server
                        </Button>

                        <Button size="lg" variant="destructive" disabled={!isRunning || isUpdating}>
                            <Square className="mr-2 h-4 w-4" />
                            Stop Server
                        </Button>

                        <Button size="lg" variant="secondary" disabled={!isRunning || isUpdating}>
                            <RefreshCw className="mr-2 h-4 w-4" />
                            Restart Server
                        </Button>

                        {!isUpdating ? (
                            <Button size="lg" variant="outline" onClick={handleUpdate}>
                                <Download className="mr-2 h-4 w-4" />
                                Update Server
                            </Button>
                        ) : (
                            <Button size="lg" variant="destructive" onClick={handleCancelUpdate}>
                                <X className="mr-2 h-4 w-4" />
                                Cancel Update
                            </Button>
                        )}
                    </div>
                </CardContent>
            </Card>

            {isUpdating && <UpdateLog logs={updateLogs} />}
        </>
    )
}

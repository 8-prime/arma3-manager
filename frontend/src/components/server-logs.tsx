import { getServerLogs } from "@/api/server-management"
import type { ServerLogEntryDTO } from "@/api/types"
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card"
import { ScrollArea } from "@/components/ui/scroll-area"
import { Terminal } from "lucide-react"
import { useEffect, useRef, useState } from "react"


export function ServerLogs() {
    const [logs, setLogs] = useState<ServerLogEntryDTO[]>([])
    const scrollRef = useRef<HTMLDivElement>(null)

    // Auto-scroll to bottom when new logs arrive
    useEffect(() => {
        let isMounted = true;

        async function fetchInfo() {
            const data = await getServerLogs();
            if (isMounted) setLogs(data);
            if (scrollRef.current) {
                scrollRef.current.scrollTop = scrollRef.current.scrollHeight
            }
        }

        fetchInfo();

        const handle = setInterval(fetchInfo, 5000);

        return () => {
            isMounted = false;
            clearInterval(handle);
        };
    }, [])

    return (
        <Card className="border-primary/20">
            <CardHeader>
                <div className="flex items-center gap-2">
                    <Terminal className="h-5 w-5 text-primary" />
                    <div>
                        <CardTitle>Server Log</CardTitle>
                        <CardDescription>Server runtime logs</CardDescription>
                    </div>
                </div>
            </CardHeader>
            <CardContent>
                <ScrollArea className="h-[300px] w-full rounded-md border border-border bg-muted/30 p-4">
                    <div ref={scrollRef} className="space-y-1">
                        {logs.map((log) => (
                            <div key={log.timestamp} className="flex items-start gap-2 font-mono text-sm">
                                <span>[{log.severity}]</span>
                                <span className="text-primary">[{new Date(log.timestamp).toLocaleTimeString()}]</span>
                                <span className="text-foreground">{log.message}</span>
                            </div>
                        ))}
                        {logs.length === 0 && (
                            <div className="text-muted-foreground font-mono text-sm">Waiting for server logs...</div>
                        )}
                    </div>
                </ScrollArea>
            </CardContent>
        </Card>
    )
}

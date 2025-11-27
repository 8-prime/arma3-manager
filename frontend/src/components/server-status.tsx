import type { ServerInfoDTO, ServerStatus } from "@/api/types"
import { Badge } from "@/components/ui/badge"
import { Activity } from "lucide-react"

export type ServerStatusProps = {
    serverInfo: ServerInfoDTO
}

const STATUS_BADGE: Record<ServerStatus,
    { label: string; variant: "default" | "destructive" | "secondary" | "outline" }
> = {
    NotInitialized: { label: "Not Initialized", variant: "secondary" },
    Initialized: { label: "Initialized", variant: "secondary" },
    Stopped: { label: "Stopped", variant: "destructive" },
    Running: { label: "Running", variant: "default" },
    Updating: { label: "Updating", variant: "outline" },
};
export function ServerStatus({ serverInfo }: Readonly<ServerStatusProps>) {
    const { label, variant } = STATUS_BADGE[serverInfo.status];
    return (
        <div className="flex items-center gap-4">
            <div className="flex items-center gap-2">
                <Activity className="h-4 w-4 text-muted-foreground" />
                <span className="text-sm text-muted-foreground">Started: {serverInfo.runningSince}</span>
            </div>
            <Badge variant={variant}>
                {label}
            </Badge>
        </div>
    )
}

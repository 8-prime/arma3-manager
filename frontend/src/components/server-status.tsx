import { Badge } from "@/components/ui/badge"
import { Activity } from "lucide-react"

export function ServerStatus() {
    const isOnline = true
    const uptime = "2h 34m"

    return (
        <div className="flex items-center gap-4">
            <div className="flex items-center gap-2">
                <Activity className="h-4 w-4 text-muted-foreground" />
                <span className="text-sm text-muted-foreground">Uptime: {uptime}</span>
            </div>
            <Badge variant={isOnline ? "default" : "destructive"} className="bg-primary text-primary-foreground">
                {isOnline ? "Online" : "Offline"}
            </Badge>
        </div>
    )
}

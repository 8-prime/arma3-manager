import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card"
import { Button } from "@/components/ui/button"
import { Input } from "@/components/ui/input"
import { Label } from "@/components/ui/label"
import { Badge } from "@/components/ui/badge"
import { Download, Play, Trash2 } from "lucide-react"
import { useState } from "react"

const SAMPLE_MISSIONS = [
    { id: 1, name: "Altis Patrol", map: "Altis", players: "1-32", active: true, size: "24.5 MB" },
    { id: 2, name: "Tanoa Liberation", map: "Tanoa", players: "1-64", active: false, size: "18.2 MB" },
    { id: 3, name: "Malden Domination", map: "Malden", players: "1-40", active: false, size: "31.8 MB" },
    { id: 4, name: "Stratis COOP", map: "Stratis", players: "1-16", active: false, size: "12.3 MB" },
]

export function MissionManager() {
    const [workshopUrl, setWorkshopUrl] = useState("")
    const [missions, setMissions] = useState(SAMPLE_MISSIONS)

    const handleDownloadFromWorkshop = () => {
        console.log("[v0] Downloading mission from workshop:", workshopUrl)
        // Mock download
        setWorkshopUrl("")
    }

    const handleActivateMission = (id: number) => {
        console.log("[v0] Activating mission:", id)
        setMissions(
            missions.map((m) => ({
                ...m,
                active: m.id === id,
            })),
        )
    }

    const handleDeleteMission = (id: number) => {
        console.log("[v0] Deleting mission:", id)
        setMissions(missions.filter((m) => m.id !== id))
    }

    return (
        <div className="grid gap-6">
            <Card>
                <CardHeader>
                    <CardTitle>Download from Workshop</CardTitle>
                    <CardDescription>Enter a Steam Workshop URL to download a mission file</CardDescription>
                </CardHeader>
                <CardContent>
                    <div className="flex gap-4">
                        <div className="flex-1">
                            <Label htmlFor="workshop-url" className="sr-only">
                                Workshop URL
                            </Label>
                            <Input
                                id="workshop-url"
                                placeholder="https://steamcommunity.com/sharedfiles/filedetails/?id=..."
                                value={workshopUrl}
                                onChange={(e) => setWorkshopUrl(e.target.value)}
                                className="font-mono text-sm"
                            />
                        </div>
                        <Button
                            onClick={handleDownloadFromWorkshop}
                            disabled={!workshopUrl}
                            className="bg-primary text-primary-foreground hover:bg-primary/90"
                        >
                            <Download className="mr-2 h-4 w-4" />
                            Download
                        </Button>
                    </div>
                </CardContent>
            </Card>

            <Card>
                <CardHeader>
                    <CardTitle>Installed Missions</CardTitle>
                    <CardDescription>Select the active mission for your server</CardDescription>
                </CardHeader>
                <CardContent>
                    <div className="space-y-3">
                        {missions.map((mission) => (
                            <div
                                key={mission.id}
                                className="flex items-center justify-between rounded-lg border border-border bg-card p-4 transition-colors hover:bg-muted/50"
                            >
                                <div className="flex items-center gap-4">
                                    <div className="flex-1">
                                        <div className="flex items-center gap-2">
                                            <h4 className="font-semibold text-foreground">{mission.name}</h4>
                                            {mission.active && <Badge className="bg-primary text-primary-foreground">Active</Badge>}
                                        </div>
                                        <div className="mt-1 flex items-center gap-3 text-sm text-muted-foreground">
                                            <span className="flex items-center gap-1">
                                                <svg className="h-4 w-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                                                    <path
                                                        strokeLinecap="round"
                                                        strokeLinejoin="round"
                                                        strokeWidth={2}
                                                        d="M9 20l-5.447-2.724A1 1 0 013 16.382V5.618a1 1 0 011.447-.894L9 7m0 13l6-3m-6 3V7m6 10l4.553 2.276A1 1 0 0021 18.382V7.618a1 1 0 00-.553-.894L15 4m0 13V4m0 0L9 7"
                                                    />
                                                </svg>
                                                {mission.map}
                                            </span>
                                            <span className="flex items-center gap-1">
                                                <svg className="h-4 w-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                                                    <path
                                                        strokeLinecap="round"
                                                        strokeLinejoin="round"
                                                        strokeWidth={2}
                                                        d="M12 4.354a4 4 0 110 5.292M15 21H3v-1a6 6 0 0112 0v1zm0 0h6v-1a6 6 0 00-9-5.197M13 7a4 4 0 11-8 0 4 4 0 018 0z"
                                                    />
                                                </svg>
                                                {mission.players}
                                            </span>
                                            <span className="flex items-center gap-1">
                                                <svg className="h-4 w-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                                                    <path
                                                        strokeLinecap="round"
                                                        strokeLinejoin="round"
                                                        strokeWidth={2}
                                                        d="M7 21h10a2 2 0 002-2V9.414a1 1 0 00-.293-.707l-5.414-5.414A1 1 0 0012.586 3H7a2 2 0 00-2 2v14a2 2 0 002 2z"
                                                    />
                                                </svg>
                                                {mission.size}
                                            </span>
                                        </div>
                                    </div>
                                </div>

                                <div className="flex items-center gap-2">
                                    {!mission.active && (
                                        <Button variant="outline" size="sm" onClick={() => handleActivateMission(mission.id)}>
                                            <Play className="mr-2 h-4 w-4" />
                                            Activate
                                        </Button>
                                    )}
                                    <Button variant="ghost" size="sm" onClick={() => handleDeleteMission(mission.id)}>
                                        <Trash2 className="h-4 w-4 text-destructive" />
                                    </Button>
                                </div>
                            </div>
                        ))}
                    </div>
                </CardContent>
            </Card>
        </div>
    )
}

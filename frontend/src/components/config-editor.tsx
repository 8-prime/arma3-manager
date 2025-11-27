"use client"

import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card"
import { Button } from "@/components/ui/button"
import { Textarea } from "@/components/ui/textarea"
import { Save, RotateCcw } from "lucide-react"
import { useState } from "react"

const DEFAULT_CONFIG = {
    hostname: "My ArmA3 Server",
    password: "",
    passwordAdmin: "adminpass123",
    maxPlayers: 32,
    motd: ["Welcome to our ArmA3 Server!", "Have fun and follow the rules."],
    voteThreshold: 0.33,
    voteMissionPlayers: 3,
    persistent: true,
    battlEye: true,
    verifySignatures: 2,
    allowedFilePatching: 1,
}

export function ConfigEditor() {
    const [config, setConfig] = useState(DEFAULT_CONFIG)
    const [rawConfig, setRawConfig] = useState("")

    const handleSave = () => {
        console.log("[v0] Saving configuration:", config)
    }

    const handleReset = () => {
        setConfig(DEFAULT_CONFIG)
    }

    const updateConfig = (key: string, value: any) => {
        setConfig({ ...config, [key]: value })
    }

    return (
        <div className="w-full">
            <Card>
                <CardHeader>
                    <CardTitle>Raw Configuration</CardTitle>
                    <CardDescription>Edit the server configuration file directly</CardDescription>
                </CardHeader>
                <CardContent>
                    <Textarea
                        value={rawConfig || JSON.stringify(config, null, 2)}
                        onChange={(e: any) => setRawConfig(e.target.value)}
                        rows={20}
                        className="font-mono text-sm"
                        placeholder="// server.cfg content..."
                    />
                    <p className="mt-2 text-xs text-muted-foreground">
                        Advanced users can edit the configuration directly. See{" "}
                        <a
                            href="https://community.bistudio.com/wiki/Arma_3:_Server_Config_File"
                            target="_blank"
                            rel="noopener noreferrer"
                            className="text-primary hover:underline"
                        >
                            ArmA3 Server Config Documentation
                        </a>
                    </p>
                </CardContent>
            </Card>

            <div className="flex items-center justify-end gap-2 pt-6">
                <Button variant="outline" onClick={handleReset}>
                    <RotateCcw className="mr-2 h-4 w-4" />
                    Reset
                </Button>
                <Button onClick={handleSave} className="bg-primary text-primary-foreground hover:bg-primary/90">
                    <Save className="mr-2 h-4 w-4" />
                    Save Configuration
                </Button>
            </div>
        </div>
    )
}

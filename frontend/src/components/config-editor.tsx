import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card"
import { Button } from "@/components/ui/button"
import { Input } from "@/components/ui/input"
import { Label } from "@/components/ui/label"
import { Textarea } from "@/components/ui/textarea"
import { Save, Plus, Trash2, CheckCircle2, Play } from "lucide-react"
import { useEffect, useState } from "react"
import type { ConfigurationBundleDto } from "@/api/types"
import { activateConfig, createConfig, deleteConfig, getActiveConfig, getConfigs, updateConfig } from "@/api/config"
import { v4 as uuidv4 } from 'uuid';

export function ConfigEditor() {
    const [configs, setConfigs] = useState<ConfigurationBundleDto[]>([])
    const [selectedConfigId, setSelectedConfigId] = useState<string | null>(configs[0]?.id || null)
    const [activeConfigId, setActiveConfigId] = useState<string | null>(configs[0]?.id || null)

    const selectedConfig = configs.find((c) => c.id === selectedConfigId)

    const handleAddConfig = async () => {
        const newConfig: ConfigurationBundleDto = {
            id: uuidv4(),
            name: "New Configuration",
            isDefault: false,
            launchParameters: "",
            serverConfig: "",
        }
        setConfigs([...configs, newConfig])
        setSelectedConfigId(newConfig.id)
        await createConfig(newConfig)
    }

    const handleDeleteConfig = async (id: string) => {
        await deleteConfig(id);
        await loadConfigs();
    }

    const updateSelectedConfig = (field: keyof ConfigurationBundleDto, value: string) => {
        if (!selectedConfig) return
        setConfigs(configs.map((c) => (c.id === selectedConfigId ? { ...c, [field]: value } : c)))
    }

    const handleActivateConfig = async (id: string) => {
        await activateConfig(id);
        setActiveConfigId((await getActiveConfig()).id)
        console.log("[v0] Activated configuration:", configs.find((c) => c.id === id)?.name)
    }

    const handleSave = async () => {
        if (!selectedConfig) {
            return;
        }
        await updateConfig(selectedConfig);
        console.log("[v0] Saving configurations:", configs)
    }

    async function loadConfigs() {
        setConfigs(await getConfigs())
        setActiveConfigId((await getActiveConfig()).id)
    }

    useEffect(() => {
        loadConfigs()
    }, [])

    return (
        <div className="flex gap-6">
            <div className="w-64 space-y-4">
                <div className="flex items-center justify-between">
                    <h3 className="text-sm font-medium">Configurations</h3>
                    <Button size="sm" variant="outline" onClick={handleAddConfig}>
                        <Plus className="h-4 w-4" />
                    </Button>
                </div>
                <div className="space-y-2">
                    {configs.map((config) => (
                        <button
                            key={config.id}
                            onClick={() => setSelectedConfigId(config.id)}
                            className={`w-full text-left rounded-lg border px-3 py-2 text-sm transition-colors ${selectedConfigId === config.id
                                ? "bg-primary/10 border-primary text-primary"
                                : "border-border hover:bg-accent"
                                }`}
                        >
                            <div className="flex items-center justify-between gap-2">
                                <span className="truncate">{config.name}</span>
                                {activeConfigId === config.id && <CheckCircle2 className="h-4 w-4 shrink-0 text-primary" />}
                            </div>
                        </button>
                    ))}
                </div>
            </div>

            <div className="flex-1 space-y-6">
                {selectedConfig ? (
                    <>
                        <Card>
                            <CardHeader>
                                <div className="flex items-center justify-between">
                                    <div>
                                        <CardTitle>Configuration Details</CardTitle>
                                        <CardDescription>Edit the selected server configuration</CardDescription>
                                    </div>
                                    {activeConfigId === selectedConfig.id ? (
                                        <div className="flex items-center gap-2 rounded-full bg-primary/10 px-3 py-1 text-sm font-medium text-primary">
                                            <CheckCircle2 className="h-4 w-4" />
                                            Active
                                        </div>
                                    ) : (
                                        <Button
                                            size="sm"
                                            variant="outline"
                                            onClick={() => handleActivateConfig(selectedConfig.id)}
                                            className="gap-2"
                                        >
                                            <Play className="h-4 w-4" />
                                            Activate
                                        </Button>
                                    )}
                                </div>
                            </CardHeader>
                            <CardContent className="space-y-4">
                                <div className="space-y-2">
                                    <Label htmlFor="configName">Configuration Name</Label>
                                    <Input
                                        id="configName"
                                        value={selectedConfig.name}
                                        onChange={(e) => updateSelectedConfig("name", e.target.value)}
                                        placeholder="Configuration name"
                                    />
                                </div>

                                <div className="space-y-2">
                                    <Label htmlFor="launchParams">Launch Parameters</Label>
                                    <Input
                                        id="launchParams"
                                        value={selectedConfig.launchParameters}
                                        onChange={(e) => updateSelectedConfig("launchParameters", e.target.value)}
                                        placeholder="-port=2302 -config=server.cfg"
                                        className="font-mono text-sm"
                                    />
                                    <p className="text-xs text-muted-foreground">Command line parameters for starting the server</p>
                                </div>

                                <div className="space-y-2">
                                    <Label htmlFor="serverConfig">Server Configuration</Label>
                                    <Textarea
                                        id="serverConfig"
                                        value={selectedConfig.serverConfig}
                                        onChange={(e) => updateSelectedConfig("serverConfig", e.target.value)}
                                        rows={20}
                                        className="font-mono text-sm"
                                        placeholder="// server.cfg content..."
                                    />
                                    <p className="text-xs text-muted-foreground">
                                        Edit the server configuration file. See{" "}
                                        <a
                                            href="https://community.bistudio.com/wiki/Arma_3:_Server_Config_File"
                                            target="_blank"
                                            rel="noopener noreferrer"
                                            className="text-primary hover:underline"
                                        >
                                            ArmA3 Server Config Documentation
                                        </a>
                                    </p>
                                </div>
                            </CardContent>
                        </Card>

                        <div className="flex items-center justify-between">
                            <Button
                                variant="destructive"
                                onClick={() => handleDeleteConfig(selectedConfig.id)}
                                disabled={configs.length === 1 || selectedConfig.isDefault}
                            >
                                <Trash2 className="mr-2 h-4 w-4" />
                                Delete Configuration
                            </Button>
                            <Button onClick={handleSave} className="bg-primary text-primary-foreground hover:bg-primary/90">
                                <Save className="mr-2 h-4 w-4" />
                                Save Configurations
                            </Button>
                        </div>
                    </>
                ) : (
                    <Card>
                        <CardContent className="flex h-64 items-center justify-center text-muted-foreground">
                            No configuration selected. Click the + button to add one.
                        </CardContent>
                    </Card>
                )}
            </div>
        </div>
    )
}

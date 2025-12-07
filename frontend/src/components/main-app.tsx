import { getServerInfo } from "@/api/server-management";
import type { ServerInfoDTO } from "@/api/types";
import { useEffect, useState } from "react";
import { ServerStatus } from "./server-status";
import { ServerControls } from "./server-controls";
import { Tabs, TabsContent, TabsList, TabsTrigger } from "./ui/tabs";
import { ConfigEditor } from "./config-editor";
import { ServerLogs } from "./server-logs";
import { ModManager } from "./mod-manager";
import { MissionManager } from "./mission-manager";

export function MainApp() {
    const [serverInfo, setServerInfo] = useState<ServerInfoDTO | undefined>();

    const updateServerInfo = async () => {
        const data = await getServerInfo();
        setServerInfo(data);
    }

    useEffect(() => {
        let isMounted = true;

        async function fetchInfo() {
            const data = await getServerInfo();
            if (isMounted) setServerInfo(data);
        }

        fetchInfo();

        const handle = setInterval(fetchInfo, 3000);

        return () => {
            isMounted = false;
            clearInterval(handle);
        };
    }, []);

    return (
        <>
            <header className="border-b border-border bg-card">
                <div className="container mx-auto px-4 py-4">
                    <div className="flex items-center justify-between">
                        <div className="flex items-center gap-3">
                            <div className="flex h-10 w-10 items-center justify-center rounded-lg bg-primary">
                                <svg className="h-6 w-6 text-primary-foreground" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                                    <path
                                        strokeLinecap="round"
                                        strokeLinejoin="round"
                                        strokeWidth={2}
                                        d="M9 3v2m6-2v2M9 19v2m6-2v2M5 9H3m2 6H3m18-6h-2m2 6h-2M7 19h10a2 2 0 002-2V7a2 2 0 00-2-2H7a2 2 0 00-2 2v10a2 2 0 002 2zM9 9h6v6H9V9z"
                                    />
                                </svg>
                            </div>
                            <div>
                                <h1 className="text-xl font-bold text-foreground">ArmA3 Server Manager</h1>
                                <p className="text-sm text-muted-foreground">Control Panel</p>
                            </div>
                        </div>
                        {!!serverInfo && <ServerStatus serverInfo={serverInfo} />}
                    </div>
                </div>
            </header>

            <main className="container mx-auto px-4 py-6">
                <div className="space-y-6">
                    <ServerControls reloadInfo={updateServerInfo} serverInfo={serverInfo} />

                    <Tabs defaultValue="config" className="w-full">
                        <TabsList className="grid w-full grid-cols-4 lg:w-auto">
                            <TabsTrigger value="config">Configuration</TabsTrigger>
                            <TabsTrigger value="server-logs">Server Logs</TabsTrigger>
                            <TabsTrigger value="mods">Mods</TabsTrigger>
                            <TabsTrigger value="missions">Missions</TabsTrigger>
                        </TabsList>
                        <TabsContent value="config" className="mt-6">
                            <ConfigEditor />
                        </TabsContent>
                        <TabsContent value="server-logs" className="mt-6">
                            <ServerLogs />
                        </TabsContent>
                        <TabsContent value="mods" className="mt-6">
                            <ModManager />
                        </TabsContent>
                        <TabsContent value="missions" className="mt-6">
                            <MissionManager />
                        </TabsContent>
                    </Tabs>
                </div>
            </main>
        </>
    )
}
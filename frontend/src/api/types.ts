export type ConfigDto = {
    configurationString: string;
}

export type ServerStatus =
    | "NotInitialized"
    | "Initialized"
    | "Stopped"
    | "Running"
    | "Updating";

export type ServerInfoDTO = {
    currentPlayers: number;
    maxPlayers: number;
    status: ServerStatus;
    runningSince: string | null; // ISO date string
    serverPath: string;
}

export type ServerLogSeverity =
    | "Info"
    | "Error";

export type ServerLogEntryDTO = {
    timestamp: string; // ISO date string
    severity: ServerLogSeverity;
    message: string;
}

export type LoadMissionRequest = {
    missionLink: string
}

export type ConfigurationBundleDto = {
    id: string,
    isDefault: boolean,
    name: string,
    serverConfig: string,
    launchParameters: string
}


export type InitializationStatus =
    | "Created"
    | "Started"
    | "Finished"
    | "Failed"

export type InitializationResource = {
    name: string
    status: InitializationStatus
}

import { uploadMission } from "@/api/missions"
import { UploadManager } from "./upload-manager"


export function MissionManager() {
    const title = 'Mission Management'
    const description = 'Upload and manage server mission'
    const action = 'Upload Mission'
    const accept = '.zip,.rar,.7z'

    return <UploadManager handleUpload={uploadMission} title={title} description={description} action={action} accept={accept} />
}

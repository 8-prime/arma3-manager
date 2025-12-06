
import { uploadMod } from "@/api/mods"
import { UploadManager } from "./upload-manager"

export function ModManager() {
    const title = 'Mod Management'
    const description = 'Upload and manage server mods'
    const action = 'Upload Mod'
    const accept = '.zip,.rar,.7z'

    return <UploadManager handleUpload={uploadMod} title={title} description={description} action={action} accept={accept} />
}

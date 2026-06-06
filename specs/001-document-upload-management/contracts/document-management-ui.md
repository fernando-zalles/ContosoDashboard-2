# Document Management UI & Service Contract

## User-facing routes

- `/documents` — My Documents list for the signed-in user
- `/documents/upload` — Document upload page with title, category, project association, tags, and file selection
- `/documents/shared` — Documents explicitly shared with the current user
- `/projects/{projectId}/documents` — Project Documents view for a selected project
- `/tasks/{taskId}/documents` — Task Documents view and attachment workflow
- `/documents/{documentId}` — Document details page with preview, edit, share, and delete actions

## Service contract

### DocumentService

#### UploadDocumentAsync
- Request: `UploadDocumentRequest`
  - `Title`: string
  - `Description`: string?
  - `Category`: string
  - `ProjectId`: int?
  - `Tags`: string?
  - `File`: IFormFile
- Response: `DocumentDto`

#### GetMyDocumentsAsync
- Request: `DocumentQuery`
  - `SortBy`: string
  - `Category`: string?
  - `ProjectId`: int?
  - `StartDateUtc`: DateTime?
  - `EndDateUtc`: DateTime?
  - `SearchText`: string?
- Response: `IEnumerable<DocumentSummaryDto>`

#### GetProjectDocumentsAsync
- Request: `projectId`, `DocumentQuery`
- Response: `IEnumerable<DocumentSummaryDto>`

#### GetSharedDocumentsAsync
- Request: `DocumentQuery`
- Response: `IEnumerable<DocumentSummaryDto>`

#### DownloadDocumentAsync
- Request: `documentId`
- Response: file stream with `ContentType`

#### PreviewDocumentAsync
- Request: `documentId`
- Response: file stream or browser-capable preview payload

#### UpdateDocumentMetadataAsync
- Request: `UpdateDocumentMetadataRequest`
  - `DocumentId`: int
  - `Title`: string
  - `Description`: string?
  - `Category`: string
  - `Tags`: string?
- Response: success/failure

#### ReplaceDocumentFileAsync
- Request: `ReplaceDocumentFileRequest`
  - `DocumentId`: int
  - `File`: IFormFile
- Response: success/failure

#### DeleteDocumentAsync
- Request: `documentId`
- Response: success/failure

#### ShareDocumentAsync
- Request: `ShareDocumentRequest`
  - `DocumentId`: int
  - `RecipientUserIds`: int[]
  - `RecipientTeam`: string?
- Response: success/failure

### IFileStorageService

- `Task<string> UploadAsync(Stream fileStream, string destinationPath, string contentType)`
- `Task DeleteAsync(string storagePath)`
- `Task<Stream> DownloadAsync(string storagePath)`
- `Task<string> GetUrlAsync(string storagePath)`

## Authorization model

- Document owners can manage their own documents.
- Project members and managers can view and download documents associated with their projects.
- Project managers can delete project documents.
- Share recipients can access shared documents and receive notifications.
- All file downloads and previews require authorization checks before returning the stream.

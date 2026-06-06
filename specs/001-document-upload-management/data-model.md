# Data Model — Document Upload and Management

## Document
Represents an uploaded document and its metadata.

- `DocumentId` (int, PK)
- `Title` (string, required, max 200)
- `Description` (string, optional, max 1000)
- `Category` (string, required, max 100)
- `Tags` (string, optional, max 500) — stored as normalized comma-separated values for search
- `OriginalFileName` (string, required, max 255)
- `ContentType` (string, required, max 255)
- `StoragePath` (string, required, max 500)
- `FileSizeBytes` (long, required)
- `UploadDateUtc` (DateTime, required)
- `UploadedById` (int, FK to `User`)
- `UploadedBy` (User relationship)
- `ProjectId` (int?, FK to `Project`)
- `Project` (Project relationship)
- `IsShared` (bool)
- `DocumentShares` (collection of `DocumentShare` records)

## DocumentShare
Represents a shared access grant for a document.

- `DocumentShareId` (int, PK)
- `DocumentId` (int, FK to `Document`)
- `Document` (Document relationship)
- `RecipientUserId` (int?, FK to `User`)
- `RecipientUser` (User relationship)
- `RecipientTeam` (string?, optional)
- `SharedByUserId` (int, FK to `User`)
- `SharedByUser` (User relationship)
- `SharedOnUtc` (DateTime, required)

## DocumentCategory
Represents the allowed category values.

- `Project Documents`
- `Team Resources`
- `Personal Files`
- `Reports`
- `Presentations`
- `Other`

> Validation rule: category values must match the predefined list.

## Relationships and Rules

- A document belongs to a single uploader (`UploadedBy`).
- A document may optionally belong to one project.
- Document owners, project members, and recipients may access a given document based on authorization rules.
- Documents stored outside `wwwroot` are served through secure endpoints.
- Deleting a document removes both the file and the metadata record.

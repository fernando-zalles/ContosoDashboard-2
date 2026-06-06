# Tasks: Document Upload and Management

**Input**: Design documents from `/specs/001-document-upload-management/`
**Prerequisites**: plan.md, spec.md, research.md, data-model.md, contracts/

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: Prepare the existing ContosoDashboard application for secure document management and local file storage.

- [ ] T001 Create document storage configuration in `ContosoDashboard/appsettings.json` and wire it into `ContosoDashboard/Program.cs`
- [ ] T002 Create `ContosoDashboard/Services/IFileStorageService.cs` for file storage abstraction
- [ ] T003 [P] Create `ContosoDashboard/Services/LocalFileStorageService.cs` to save, delete, and load files from `AppData/uploads`
- [ ] T004 [P] Create `ContosoDashboard/Models/Document.cs` for document metadata and `ContosoDashboard/Models/DocumentShare.cs` for shared access records
- [ ] T005 Update `ContosoDashboard/Data/ApplicationDbContext.cs` to register `DbSet<Document>` and `DbSet<DocumentShare>` and configure the Document model property constraints
- [ ] T006 [P] Add document category validation constants and supported MIME/type whitelist helpers in `ContosoDashboard/Services/DocumentValidationService.cs`

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Implement core document management services, file security, and authorization integration before user story work begins.

- [ ] T007 Implement `ContosoDashboard/Services/DocumentService.cs` with upload, metadata persistence, download, preview, update, replace, delete, search, and share support
- [ ] T008 Update `ContosoDashboard/Services/NotificationService.cs` to emit notifications for document shares and project document events
- [ ] T009 Add secure file serving support in `ContosoDashboard/Pages/Documents/Download.razor.cs` and `ContosoDashboard/Pages/Documents/Preview.razor.cs`
- [ ] T010 Add authorization guards in `ContosoDashboard/Services/DocumentService.cs` to enforce owner, project membership, and share recipient access rules
- [ ] T011 Add database migrations or seed script notes for documents in `ContosoDashboard/Data/ApplicationDbContext.cs`
- [ ] T012 [P] Create `ContosoDashboard/Pages/Documents/_DocumentLayout.razor` or shared component styles for document management pages

**Checkpoint**: Core document management infrastructure is complete and blocks all user stories until done.

---

## Phase 3: User Story 1 - Upload and organize documents (Priority: P1)

**Goal**: Enable users to upload documents with metadata, browse their uploads, sort, filter, and search their documents.

**Independent Test**: Verify a user can upload a supported file, add required metadata, and immediately see the document in their My Documents list with sortable/filterable fields.

- [ ] T013 [US1] Create `ContosoDashboard/Pages/Documents/Upload.razor` with file selection, title, category, optional project selection, tags, and upload progress UI
- [ ] T014 [US1] Create `ContosoDashboard/Pages/Documents/MyDocuments.razor` showing the signed-in user's documents with title, category, upload date, file size, associated project, sorting, and filtering controls
- [ ] T015 [US1] Implement search and filter query handling in `ContosoDashboard/Services/DocumentService.cs` for title, description, tags, uploader, project, category, and date range
- [ ] T016 [US1] Add `Documents` navigation links in `ContosoDashboard/Shared/NavMenu.razor` and `ContosoDashboard/Pages/Index.razor` for the new document pages
- [ ] T017 [US1] Add a recent documents widget and document count summary to `ContosoDashboard/Pages/Index.razor`
- [ ] T018 [P] [US1] Create `ContosoDashboard/Pages/Documents/SharedWithMe.razor` to display documents shared directly with the current user

**Checkpoint**: User Story 1 can be verified independently by uploading a document and browsing the My Documents page.

---

## Phase 4: User Story 2 - Access and manage owned documents (Priority: P2)

**Goal**: Allow document owners and authorized project users to preview, download, edit metadata, replace files, and delete documents.

**Independent Test**: Verify a user can open a document detail page, update metadata, replace the file content, and delete the document with confirmation.

- [ ] T019 [US2] Create `ContosoDashboard/Pages/Documents/DocumentDetails.razor` with preview, download, edit metadata, replace file, delete, and sharing actions
- [ ] T020 [US2] Implement metadata editing and file replacement in `ContosoDashboard/Services/DocumentService.cs`
- [ ] T021 [US2] Add a delete confirmation workflow and permanent removal logic in `ContosoDashboard/Services/DocumentService.cs`
- [ ] T022 [US2] Update `ContosoDashboard/Pages/Projects/ProjectDetails.razor` to show associated project documents and allow project members to view/download them
- [ ] T023 [US2] Add authorization and preview/download file access rules in `ContosoDashboard/Pages/Documents/Preview.razor.cs` and `ContosoDashboard/Pages/Documents/Download.razor.cs`
- [ ] T024 [P] Add `ContosoDashboard/Pages/Documents/DocumentHistory.razor` or inline details into `DocumentDetails.razor` to surface upload metadata, history, and access details

**Checkpoint**: User Story 2 is complete when document owners can manage files and authorized users can access them securely.

---

## Phase 5: User Story 3 - Share and integrate documents (Priority: P3)

**Goal**: Enable sharing documents with users or teams, attach documents to tasks, and notify recipients about shared/project documents.

**Independent Test**: Verify a user can share a document with another user, the recipient gets an in-app notification, and the shared document appears in the recipient's Shared With Me list.

- [ ] T025 [US3] Extend `ContosoDashboard/Pages/Documents/DocumentDetails.razor` with share controls for user and team recipient selection
- [ ] T026 [US3] Add `DocumentShare` persistence and recipient/team sharing logic in `ContosoDashboard/Services/DocumentService.cs`
- [ ] T027 [US3] Emit shared document notifications from `ContosoDashboard/Services/NotificationService.cs`
- [ ] T028 [US3] Update `ContosoDashboard/Pages/Tasks/TaskDetails.razor` to attach a document to a task and persist the task association in `ContosoDashboard/Services/TaskService.cs`
- [ ] T029 [US3] Ensure document attachments inherit the task's project association in `ContosoDashboard/Services/DocumentService.cs`
- [ ] T030 [P] [US3] Add a `Shared With Me` filter and project document filter to the document search UI in `ContosoDashboard/Pages/Documents/MyDocuments.razor`

**Checkpoint**: User Story 3 is complete when sharing, task attachments, and notifications are working end to end.

---

## Phase 6: Polish & Cross-Cutting Concerns

**Purpose**: Stabilize the feature across all user stories, add tests, documentation, and cleanup.

- [ ] T031 [P] Create unit tests for `ContosoDashboard/Services/DocumentService.cs` and `ContosoDashboard/Services/LocalFileStorageService.cs`
- [ ] T032 [P] Create integration tests for document upload, download, preview, edit, delete, and share scenarios
- [ ] T033 [P] Update `specs/001-document-upload-management/quickstart.md` with any final developer verification steps
- [ ] T034 [P] Update `README.md` or training documentation with a summary of the document management feature and user flow
- [ ] T035 [P] Remove or refactor any temporary test scaffolding and apply formatting across new files
- [ ] T036 [P] Validate that document upload, search, preview, and access workflows meet performance and security acceptance criteria

---

## Dependencies & Execution Order

### Phase Dependencies

- **Setup (Phase 1)**: No dependencies; can begin immediately
- **Foundational (Phase 2)**: Depends on Setup completion; blocks all user story work until done
- **User Stories (Phase 3+)**: Depend on Foundational completion; can proceed in parallel once foundation is ready
- **Polish (Phase 6)**: Depends on User Stories completion

### User Story Dependencies

- **US1**: Depends on foundational document storage and upload services; otherwise independent
- **US2**: Depends on US1 plus secure file serving and metadata update support
- **US3**: Depends on US2 and document sharing/notification infrastructure

### Parallel Opportunities

- `T003` and `T004` can run in parallel because they create independent service and model files
- `T011`, `T012`, and `T016` can run in parallel after foundational tasks are complete because they work on different UI pages and search logic
- `T031`, `T032`, `T033`, `T034`, `T035`, and `T036` can run in parallel as polish and validation tasks

## Implementation Strategy

### MVP First

1. Complete Phase 1: Setup
2. Complete Phase 2: Foundational
3. Complete Phase 3: User Story 1
4. Validate User Story 1 independently
5. Continue to User Story 2 and 3 once the upload/browse workflow is stable

### Incremental Delivery

- Deliver upload and browsing first, then document management, then sharing and integration
- Keep each story independently testable and stop at checkpoints for verification
- Prefer a working demo after US1 before adding US2 and US3

---

## Feature Summary

- **P1**: Upload, browse, sort, filter, and search owned documents
- **P2**: Preview, download, edit, replace, delete, and manage documents securely
- **P3**: Share with users/teams, attach to tasks, and notify recipients

# Research — Document Upload and Management

## Decision: Local filesystem storage with service abstraction
**Decision:** Store uploaded files in a dedicated local directory under `AppData/uploads` outside `wwwroot`, and access them through a service abstraction.

**Rationale:** The stakeholder requirement is explicitly offline training support. Local storage preserves that requirement while allowing a clean migration path to cloud storage via `IFileStorageService`.

**Alternatives considered:**
- Azure Blob Storage / cloud file service: rejected because the application must work without cloud dependencies for training.
- Database BLOB storage: rejected to avoid large binary storage in LocalDB and to keep file preview/download logic simple.

## Decision: Integer document IDs and text category values
**Decision:** Use integer `DocumentId` keys consistent with existing app entities, and store category values as text strings.

**Rationale:** Existing application conventions use integer primary keys for User/Project entities. Text categories simplify the schema and avoid enum migration complexity while still enabling validation by allowed values.

**Alternatives considered:**
- GUID identifiers: rejected because the spec explicitly aligns with existing integer key conventions.
- Enum-backed category values: rejected due to the stated preference for text values and training simplicity.

## Decision: Blazor Server UI with secure download endpoints
**Decision:** Implement document management as new Blazor Server pages and components, with backend endpoints serving files from outside `wwwroot`.

**Rationale:** This aligns with the current ContosoDashboard architecture and ensures authorization checks can be enforced before file downloads or previews.

**Alternatives considered:**
- Separate SPA frontend + API backend: rejected because the current project is a Blazor Server app and the feature should fit the existing architecture.
- Direct file links into `wwwroot`: rejected because it would bypass required authorization checks and violate security guidance.

## Decision: Document sharing with explicit recipient records
**Decision:** Represent shares using a `DocumentShare` entity that links documents to recipient users or recipient team identifiers.

**Rationale:** This model supports the requirement to share with specific users or teams and allows auditability and notification integration.

**Alternatives considered:**
- Project-level sharing only: rejected because the feature explicitly requires user/team-level sharing.
- One-size-fits-all access flags: rejected because it would not support explicit recipient history and notification semantics.

## Decision: Search and filtering within authorized scope
**Decision:** Implement document search and filtering at the database layer, ensuring queries only return documents the current user may access.

**Rationale:** The feature requires search results within 2 seconds and strict permission enforcement. Database-level filtering is the safest and most efficient option for the training scenario.

**Alternatives considered:**
- Client-side filtering after loading all documents: rejected because it would not scale and could expose unauthorized documents.
- External search engine: rejected because it introduces unnecessary complexity and cloud dependency.

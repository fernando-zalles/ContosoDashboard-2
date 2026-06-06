<!--
Sync Impact Report
Version change: none -> 1.0.0
Modified principles:
- I. Training-First Transparency
- II. Security by Design
- III. Offline-First Architecture
- IV. Specification-Led Development
- V. Simplicity & Maintainability
Added sections:
- Training & Safety Constraints
- Review & Delivery Process
Templates requiring updates:
- .specify/templates/plan-template.md ✅ reviewed
- .specify/templates/spec-template.md ✅ reviewed
- .specify/templates/tasks-template.md ✅ reviewed
- .specify/templates/constitution-template.md ⚠ pending (template remains generic)
Follow-up TODOs: none
-->

# ContosoDashboard Constitution

## Core Principles

### I. Training-First Transparency
The project MUST clearly declare its educational, training-only purpose. All features, security controls, and architectural decisions are validated as instructional examples rather than production-ready solutions.

### II. Security by Design
Security controls MUST be enforced at every layer, even in a training context: authentication, authorization, IDOR protection, and data isolation are non-negotiable. Changes MUST preserve mock security guarantees and not weaken authorization checks for convenience.

### III. Offline-First Architecture
The application MUST remain runnable without cloud dependencies while maintaining clear abstraction boundaries for future cloud migration. Infrastructure abstractions and dependency injection MUST enable swapping local training implementations for production services with minimal business logic changes.

### IV. Specification-Led Development
All substantive work MUST be driven by written specifications, plans, and task artifacts. Requirements, user stories, and success criteria MUST be documented before implementation, and reviews MUST verify constitution compliance.

### V. Simplicity & Maintainability
Features MUST be kept small, readable, and maintainable; avoid unnecessary complexity, premature optimization, and over-engineered solutions. Code SHOULD favor clarity, explicit intent, and consistency with the existing Blazor Server / ASP.NET Core structure.

## Training & Safety Constraints
The repository is intended for instruction and offline training only. It MUST NOT be treated as a production-grade application, and any feature changes MUST preserve the distinction between mock training behavior and production requirements. External service integration, cloud dependencies, and production security controls are explicitly out of scope unless a clear migration path is documented.

## Review & Delivery Process
Every change MUST be supported by:
- a written feature specification or task definition,
- a lightweight review of design tradeoffs,
- a validation step that verifies the change preserves training safety and security constraints.

Pull requests MUST include a short compliance summary referencing the relevant principles, and reviewers MUST confirm that the work does not violate offline training assumptions or mock authentication guarantees.

## Governance
This constitution supersedes informal practices and governs all project decisions for ContosoDashboard. Amendments require written documentation, approval by the project owner or training lead, and an update to this constitution file. Changes that alter core principles, training scope, or security assumptions MUST be versioned and tracked explicitly.

**Version**: 1.0.0 | **Ratified**: 2026-06-06 | **Last Amended**: 2026-06-06


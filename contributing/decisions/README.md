# Architecture Decision Records (ADR)

An Architecture Decision Record (ADR) captures an important architecture decision along with its context, rationale, and alternatives.

## Conventions

- Directory: `contributing/decisions`
- Naming: `YYYY-MM-DD-choose-database.md`
- Format: Context and Problem Statement, Decision, Alternatives Considered, and optional More Information
- Omit YAML frontmatter and explicit status fields; merging the ADR pull request accepts the decision
- Include tradeoffs in the decision and alternatives; keep implementation and verification in issues or PRs

## Workflow

- Propose a new ADR in a pull request and discuss it there.
- Merging the pull request accepts the decision; an explicit status is unnecessary.
- If replaced later: create a new ADR and link both records, explaining the replacement.

## ADRs

- [Adopt architecture decision records](2026-11-15-adopt-architecture-decision-records.md) (2026-11-15)
- [Build on ASP.NET Core with EF Core](2026-11-15-build-on-aspnet-core-with-ef-core.md) (2026-11-15)
- [Implement reports and authentication per assignment](2026-11-15-implement-reports-and-authentication.md) (2026-11-15)
- [Organize repository into backend and frontend folders](2026-11-16-organize-repository-into-backend-and-frontend.md) (2026-11-16)
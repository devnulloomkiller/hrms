# Adopt architecture decision records

## Context and Problem Statement

Architecture decisions in this project are made implicitly — through code, conversations, and tribal knowledge. When a new contributor (human or AI agent) joins the codebase, there is no record of *why* things are built the way they are. This makes it hard to:

- Understand whether a pattern is intentional or accidental
- Know if a past decision still applies or has been superseded
- Avoid relitigating decisions that were already carefully considered

We need a lightweight, version-controlled way to capture decisions where the code lives.

## Decision

Adopt Architecture Decision Records (ADRs), stored in `contributing/decisions/`.

Conventions:

- One ADR per file, named `YYYY-MM-DD-title-with-dashes.md`
- An ADR is accepted when its pull request merges; an explicit status is unnecessary
- Superseded ADRs link to their replacement
- Use Context and Problem Statement, Decision, Alternatives Considered, and optional More Information
- Omit YAML frontmatter and explicit status fields
- Include tradeoffs with the decision and alternatives; keep implementation plans and verification in issues or PRs
- ADRs are self-contained so contributors and coding agents can understand the decision and its rationale

Decisions are discoverable and version-controlled alongside the code, helping contributors understand the rationale and avoid relitigating settled questions. Writing records takes time, and they need periodic review to mark outdated decisions as deprecated or superseded.

## Alternatives Considered

- No formal records: Continue making decisions in conversations and code comments. Rejected because context is lost and decisions get relitigated.
- Wiki or Notion pages: Capture decisions outside the repo. Rejected because they drift out of sync with the code and are not version-controlled.
- Lightweight RFCs: A more heavyweight process with formal review cycles. Rejected as overkill for most decisions; ADRs can expand the analysis when needed.

## More Information

- Michael Nygard, "Documenting Architecture Decisions": <https://cognitect.com/blog/2011/11/15/documenting-architecture-decisions>

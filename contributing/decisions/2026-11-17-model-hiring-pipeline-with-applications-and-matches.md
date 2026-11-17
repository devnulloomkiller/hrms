# Model hiring with applications and resume-vacancy matches

## Context and Problem Statement

The initial schema attached a resume to one vacancy (`Resumes.VacancyId`) and allowed many resumes per candidate. That model cannot answer two everyday HR questions: which vacancies a candidate is actually being considered for, and which resumes HR shortlisted for a vacancy by matching requirements. A resume also changed meaning every time it was moved to another vacancy, so application history was lost.

The product needs three relationships with different semantics:

- a candidate applies to one or many vacancies and moves through a hiring stage;
- a candidate has exactly one resume;
- a resume can be matched to many vacancies, with filters and a recorded match result.

## Decision

Introduce two link entities and make the resume a candidate profile:

1. **`Resume` is 1:1 with `Candidate`.** `Resumes.CandidateId` is a unique foreign key. The resume is edited together with the candidate card and contains headline, summary, experience, desired salary and `ResumeDetails`.
2. **`JobApplication` is the N:N link between candidate and vacancy.** It carries `Stage` (`Applied`, `Screening`, `Interview`, `Offer`, `Hired`, `Rejected`, `Withdrawn`), `AppliedAt`, `UpdatedAt`, source and notes. A candidate can have only one application per vacancy (unique `CandidateId + VacancyId`).
3. **`ResumeVacancyMatch` is the N:N link between resume and vacancy.** It carries `MatchStatus` (`Suggested`, `Shortlisted`, `Rejected`), `MatchScore` 0–100, `MatchedAt` and an optional comment. The score is computed from experience, required skills and salary fit when a match is saved.
4. **`Interview` belongs to a `JobApplication`** instead of pointing at a resume and a vacancy separately. Candidate and vacancy are derived from the application, so they cannot disagree. Scheduling an interview advances an application from `Applied` or `Screening` to `Interview`.
5. **Matching is filter-driven and human-confirmed.** Vacancy fields `MinExperience` and `RequiredSkills` drive the candidate pool; filters narrow it by experience, skill, desired salary, vacancy activity and existing links. HR selects resumes and saves them as a match. Nothing is auto-linked.

The UI follows the semantics: a vacancy page shows its applications and its shortlist; a candidate page shows one resume, applications and matched vacancies; a separate applications board shows the pipeline by stage.

Demo data is seeded on startup when the candidate and vacancy tables are empty, so a fresh checkout shows a complete workflow without importing SQL manually.

### Non-goals

- Automatic sourcing or auto-advancing candidates through stages.
- Multiple resumes per candidate or resume versioning.
- Stage history/audit log. `UpdatedAt` only records the latest change.

### Accepted tradeoffs

- Two N:N links look redundant on paper. They are kept separate because an application means "candidate applied" while a match means "HR shortlisted the resume"; merging them would lose the shortlist-before-application flow.
- `MatchScore` is denormalized. It is recalculated when a match is created or refreshed, not on every resume edit, so a score can become stale until the next save.
- The migration from the old schema clears old interviews because they cannot be mapped to an application without ambiguity. This is acceptable for the educational demo database; real data needs a dedicated mapping script.

## Alternatives Considered

**Keep `Resumes.VacancyId` and add `Candidates.VacancyId`.** Simpler CRUD, but a candidate would be pinned to one vacancy and the resume would still be an application rather than a profile. This loses multi-vacancy applications and history.

**One `Application` entity for both application and shortlist.** Fewer tables, but an application requires a stage and an interview, while a shortlist may exist before the candidate applies. Overloading one row would make `Stage` nullable or invent meaningless stages for shortlisted-but-not-applied resumes.

**Auto-generate matches and applications from scoring.** Less clicking, but HR loses control over the shortlist and the system creates applications the candidate never made. Filtered suggestions with explicit confirmation keep the human in the loop.

**Keep interviews linked to resume and vacancy separately.** It preserves the old queries, but the two FKs can drift (interview for a vacancy the resume is not linked to) and every interview needs a duplicate consistency check. Linking to the application removes that class of bugs.

## More Information

- Schema and relationships: `docs/database-schema.md`
- Migration: `backend/Hrms.Infrastructure/Migrations/20261117175157_RestructureHiringPipeline.cs`
- Matching rules: `backend/Hrms.Application/Services/ResumeService.cs`
- Demo seed: `backend/Hrms.Infrastructure/DbInitializer.cs`

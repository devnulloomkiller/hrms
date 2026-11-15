# HRMS

<!-- adr-skill:start -->
## adr-skill
For architecture decisions (capturing a decision with rationale, checking decisions before changes, ADR lifecycle), read `.agents/skills/adr-skill/SKILL.md`. ADRs live in `contributing/decisions/`. Confirm with the user before writing or editing an ADR unless explicitly asked. Do not use for routine bug fixes or single-layer changes.
<!-- adr-skill:end -->

<!-- ui-ux-pro-max:start -->
## ui-ux-pro-max
For UI/UX work in this project (designing or reviewing pages, components, design systems, accessibility, typography, color, charts), read `.agents/skills/ui-ux-pro-max/SKILL.md` and use its search script (`python3 .agents/skills/ui-ux-pro-max/scripts/search.py`) to query the catalogs before proposing designs. Runs alongside antislop: this skill supplies direction and data, antislop filters slop. UI here is server-rendered MVC (Razor + Bootstrap), not React — ignore React/React Native defaults in results.
<!-- ui-ux-pro-max:end -->

<!-- session:start -->
## Session context
At session start, read in order: `CHANGELOG.md` (what is done, what is planned), `docs/requirements.md` (assignment status), `contributing/decisions/README.md` (ADR index). Update CHANGELOG and requirements status whenever a work item is completed. New changelog entries go on top under [Unreleased] or a dated heading; move planned-but-not-done items back into the plan list.
<!-- session:end -->

<!-- antislop:start -->
## antislop
For UI, copy, people, mobile layout, or code comments work, read `antislop.md` (core) and then the skill for the task:
- UI / visual: `skills/antislop-ui/SKILL.md`
- Copy & text: `skills/antislop-copywriting/SKILL.md`
- People: `skills/antislop-human/SKILL.md`
- Mobile / responsive: `skills/antislop-layoutmobile/SKILL.md`
- Code comments: `skills/antislop-code/SKILL.md`
Before starting, follow the core's "Two Usage Modes" section in strict order: explicit session instruction first, then global preference, then ask. A session instruction always wins. For a resolved mode, say `antislop active: <mode> (session override).` or `antislop active: <mode> (global preference).` once before presenting findings or making edits, using the actual mode and source. Acknowledging the user's request without naming the source does not replace this notice.
Only an explicit choice of antislop during or after selects a session mode. A request to review, audit, or avoid file edits does not select a mode; read the global preference in that case. Another skill's mode does not select antislop's mode.
If the mode is unresolved, ask during/after and end the response; wait for the answer before any UI review, planning, or concept. For read-only tasks, put the active-mode notice only at the start of the final answer, never in progress messages. For editing tasks, announce before the first edit and omit it from the final answer.
To update antislop later: download `antislop.md` again, or run `npx antislop-ai --update` if it was installed as skill folders.
<!-- antislop:end -->

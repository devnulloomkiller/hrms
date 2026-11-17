# Design System Master File — HRMS

> Источник правды по UI. Реализация токенов — `frontend/Hrms.Web/wwwroot/css/site.css`.
> Для страницы сначала проверьте `design-system/hrms/pages/[page-name].md`; если файла нет, действуют правила ниже.

**Project:** HRMS — кадровый учёт и найм
**Direction:** монохромный рабочий стол HR; светлая тема по умолчанию, тёмная — переключателем
**Updated:** 2026-11-17

## Global Rules

### Color Palette

Палитра намеренно нейтральная: цвет не конкурирует с данными, а статусы всегда дублируются текстом.

| Role | Light | Dark | CSS Variable |
|------|-------|------|--------------|
| Primary / ink | `#111111` | `#FFFFFF` | `--color-primary` |
| On primary | `#FFFFFF` | `#000000` | `--color-on-primary` |
| Background | `#F7F7F7` | `#000000` | `--color-background` |
| Card | `#FFFFFF` | `#111111` | `--color-card` |
| Foreground | `#111111` | `#F5F5F5` | `--color-foreground` |
| Muted | `#EFEFEF` | `#1F1F1F` | `--color-muted` |
| Muted foreground | `#595959` | `#B0B0B0` | `--color-muted-foreground` |
| Border | `#DDDDDD` | `#333333` | `--color-border` |
| Border strong | `#A3A3A3` | `#666666` | `--color-border-strong` |
| Soft accent | `#E8E8E8` | `#262626` | `--color-soft-accent` |
| Soft danger | `#EFEFEF` | `#1F1F1F` | `--color-soft-danger` |
| Soft warning | `#DFDFDF` | `#303030` | `--color-soft-warning` |

No brand gradient, no colored glow, no purple/blue default. Status pills use neutral tints plus a text label; `pill-hired` may invert to primary ink.

### Typography

- **UI / body:** Plus Jakarta Sans, weights 400–800.
- **Dates, sums, scores:** Fira Code, tabular numbers.
- **Headings:** Plus Jakarta Sans 600–700, balanced wrapping; page title `clamp(1.75rem, 3vw, 2.4rem)`.
- Body 15px, line-height 1.55; helper text 12–13px and never below AA contrast.

### Layout & Spacing

| Token | Value | Usage |
|-------|-------|-------|
| `--radius-sm` | 6px | pills, small controls |
| `--radius-md` | 10px | cards, inputs |
| `--radius-lg` | 16px | large surfaces |
| sidebar | 232px desktop | fixed left navigation |
| app container | `min(100% - 3rem, 1320px)` | workspace width |

Use a 4/8px rhythm. Tables and forms live in cards with 1px borders and no elevation; shadow is reserved for dropdowns.

## Components

### Buttons and links

- Minimum 44px height; focus ring `3px` with offset, visible in both themes.
- Primary action: ink background + white text. Secondary: outline. Danger: outline text button; destructive confirmation is a separate page.
- Every clickable element gets `cursor: pointer`; disabled gets `not-allowed` and reduced emphasis.

### Forms

- Label above input; helper text below; inline validation next to the field and an error summary at the top with `role="alert"`.
- Keep server-side validation; client validation is progressive enhancement.
- Date inputs use `date` / `datetime-local`; decimal money inputs use `step="0.01"` (jQuery step validator rejects `.00` with `step="1000"`).
- Combined candidate form edits contacts and the single resume together; do not offer a second resume CRUD.

### Tables

- Wrap every table in `.table-wrap`; on mobile use `.responsive-table` + `data-label` to turn rows into cards.
- Header is muted with 12px uppercase-free labels; numeric columns use tabular figures.
- Empty state explains what to do next and includes the action, not just “0 results”.

### Filters

- Filter bar uses a card, visible labels, and URL query parameters so the state is shareable.
- Checkboxes are real form controls; on mobile they stack full-width and stay tappable.
- “Nothing found” state offers a reset action; it never dead-ends.

### Pipeline board

- Stage columns: Новые, Скрининг, Собеседования, Оффер, Наняты; closed applications live in a native `<details>`.
- Card shows candidate, vacancy, experience, applied date and interview count; “Открыть отклик” is the action.
- Desktop: five columns; mobile: one column per stage (no hidden horizontal content).

### Match score

- Show a number and a small meter, never color alone. Score 0–100: experience 40, required skills 40, salary fit 20.
- Filtering is a human action: suggestions are not auto-linked.

## UX Rules

- Every list has a useful empty state, and every form has a way back.
- Success and error after a form submit are shown as a flash message (`role="status"` / `role="alert"`).
- Do not delete data silently: deleting a vacancy with applications is blocked with an explanation; deleting a candidate warns about cascading applications and interviews.
- Scheduling an interview advances an application from “Новый отклик”/“Скрининг” to “Собеседование”.
- Keep keyboard focus visible, preserve source order, and respect `prefers-reduced-motion`.
- Icons are inline SVG, 1.8px stroke, `aria-hidden` when decorative; no emoji as UI icons.

## Anti-Patterns

- Multi-color status palettes, gradients, glassmorphism on every surface, colored left stripes, decorative dots.
- Hidden filters, filter bars with labels only as placeholders, tables that overflow the page instead of their wrapper.
- Toasts as the only error feedback, English validation messages in Russian UI, `outline: none` without replacement.
- Cards inside cards inside cards; decorative dashboards with metrics that do not link to the data behind them.

## Pre-Delivery Checklist

- [ ] Light and dark themes checked independently; text contrast ≥ 4.5:1, borders/focus ≥ 3:1.
- [ ] 375px: no horizontal page overflow; tables are cards or scroll inside `.table-wrap`; pipeline stages stack.
- [ ] All touch targets ≥ 44px and not crowded.
- [ ] Labels, hints, error summary and inline errors are present; errors announced.
- [ ] Empty, loading and error states exist where data loads.
- [ ] `prefers-reduced-motion` respected.
- [ ] Icons are SVG, consistent stroke, decorative ones hidden from screen readers.
- [ ] Filters and current state reflected in the URL.

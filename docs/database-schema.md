# Схема базы данных HRMS

СУБД: SQLite (EF Core-миграции дублируются скриптами в `database/`).

## Таблицы и связи

| Связь | Тип | Реализация | Поведение при удалении |
|---|---|---|---|
| Candidate → Resume | 1:N | `Resumes.CandidateId` FK | CASCADE |
| Vacancy → Resume | 1:N (опционально) | `Resumes.VacancyId` FK NULL | SET NULL |
| Resume ↔ ResumeDetails | 1:1 | `ResumeDetails.ResumeId` — PK и FK одновременно | CASCADE |
| Resume → Interview | 1:N | `Interviews.ResumeId` FK | CASCADE |
| Vacancy → Interview | 1:N | `Interviews.VacancyId` FK | RESTRICT |
| Interview ↔ Interviewer | N:N | junction `InterviewInterviewers` (составной PK InterviewId+InterviewerId, атрибут Role) | CASCADE в обе стороны |

## Ключи и ограничения (сводно)

| Таблица | PK | FK | NOT NULL | Уникальные | CHECK |
|---|---|---|---|---|---|
| Candidates | Id | — | LastName, FirstName, Email | Email | длина имён ≥ 2 |
| Vacancies | Id | — | Title, Department, SalaryMin, Status, PublishedAt | — | SalaryMax ≥ SalaryMin; Status/диапазоны |
| Resumes | Id | CandidateId, VacancyId (null) | CandidateId, Summary, ExperienceYears, SubmittedAt, Status | — | Status, ExperienceYears ≥ 0, длина Summary |
| ResumeDetails | ResumeId (=FK) | ResumeId | ResumeId | — (PK) | — |
| Interviews | Id | ResumeId, VacancyId | ResumeId, VacancyId, ScheduledAt, Mode, Result | — | Mode, Result ∈ enum |
| Interviewers | Id | — | FullName | Email | длина FullName |
| InterviewInterviewers | (InterviewId, InterviewerId) | InterviewId, InterviewerId | оба PK, Role | — (составной PK запрещает дубли участника) | длина Role |

Индексы: `Resumes.Status`, `Resumes.CandidateId`, `Interviews.ScheduledAt`, уникальные — `Candidates.Email`, `Interviewers.Email`.

Таблицы Identity (AspNetUsers/AspNetRoles/…) создаются EF Core-миграцией, в скрипты не включены.

## Скрипты

```bash
# создание
sqlite3 hrms.db < database/01_create_tables.sql
# заполнение
sqlite3 hrms.db < database/02_seed_data.sql
```

Проверено (2026-11-15): create+seed проходят без ошибок; счётчики: 5 кандидатов, 5 вакансий, 9 резюме, 5 деталей, 8 собеседований, 5 интервьюеров, 12 участников.

## ER-диаграмма (текстовая)

```
Candidates 1───N Resumes N───1 Vacancies
                  │ 1                 │ 1
                  │                   │
                  ▼ 1:1               ▼ 1:N
            ResumeDetails         Interviews
                                      │ N
                                      ▼
                          InterviewInterviewers ←─ N ── Interviewers
```

## Соответствие модели EF Core

Те же сущности в `backend/Hrms.Domain/Entities/` + fluent-конфигурация в `ApplicationDbContext`. Миграция `InitialCreate` — до изменения схемы (ResumeDetails/Interviewer/InterviewInterviewers добавлены позже, нужна новая миграция).

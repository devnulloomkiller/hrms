# Схема базы данных HRMS

СУБД: MySQL 8.4 в Docker (EF Core-миграции создают схему при старте приложения; скрипты в `database/` повторяют логическую схему).

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
docker exec -i hrms-db mysql -uroot -phrms_root hrms < database/01_create_tables.sql
# заполнение
docker exec -i hrms-db mysql -uhrms -phrms hrms < database/02_seed_data.sql
```

Проверено (2026-11-16, MySQL 8.4 в контейнере): create+seed проходят без ошибок; счётчики: 5 кандидатов, 5 вакансий, 9 резюме, 5 деталей, 8 собеседований, 5 интервьюеров, 12 участников; JOIN через junction-таблицу корректен.

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

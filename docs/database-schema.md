# Схема базы данных HRMS

СУБД: MySQL 8.4 в Docker. Схему создают EF Core-миграции при старте приложения; скрипты в `database/` повторяют её для ручной демонстрации.

## Модель найма

Кандидат — это человек с контактами и **одним** резюме. Отклик связывает кандидата с вакансией и ведёт его по воронке. Резюме можно отдельно подобрать к нескольким вакансиям: подбор хранит оценку соответствия и статус.

| Связь | Тип | Реализация | Поведение при удалении |
|---|---|---|---|
| Candidate ↔ Resume | 1:1 | `Resumes.CandidateId` — уникальный FK | CASCADE |
| Candidate ↔ Vacancy | N:N | `JobApplications` (отклик): Stage, AppliedAt, Source, Notes | кандидат → CASCADE, вакансия → RESTRICT |
| Resume ↔ Vacancy | N:N | `ResumeVacancyMatches` (подбор): Status, MatchScore, MatchedAt, Comment | CASCADE в обе стороны |
| Resume ↔ ResumeDetails | 1:1 | `ResumeDetails.ResumeId` — PK и FK одновременно | CASCADE |
| JobApplication → Interview | 1:N | `Interviews.ApplicationId` FK | CASCADE |
| Interview ↔ Interviewer | N:N | `InterviewInterviewers` (составной PK, атрибут Role) | CASCADE в обе стороны |

### Почему две связи N:N

- **Отклик** (`JobApplications`) — кандидат реально подался на вакансию; у отклика есть этап, дата и собеседования. Один кандидат может откликнуться на одну вакансию только один раз (уникальный `CandidateId + VacancyId`).
- **Подбор** (`ResumeVacancyMatches`) — HR сохранил резюме в шортлист вакансии. Подбор не заменяет отклик: можно подобрать резюме и ещё не создать отклик. Обе связи независимы, поэтому в интерфейсе статус подбора и этап отклика показываются раздельно.

### Оценка соответствия

`MatchScore` — целое от 0 до 100, считается в `ResumeService.MatchScoring`:

- 40 баллов — опыт: `min(1, experience / vacancy.MinExperience) * 40`;
- 40 баллов — обязательные навыки: доля совпавших навыков из `Vacancy.RequiredSkills` относительно `ResumeDetails.Skills`;
- 20 баллов — зарплата: полный балл, если желаемая зарплата не выше `SalaryMax`, иначе пропорционально.

Оценка пересчитывается при каждом сохранении подбора и не хранится как источник истины для отбора.

## Ключи и ограничения (сводно)

| Таблица | PK | FK | NOT NULL | Уникальные | CHECK |
|---|---|---|---|---|---|
| Candidates | Id | — | LastName, FirstName, Email | Email | — |
| Vacancies | Id | — | Title, Department, SalaryMin, MinExperience, Status, PublishedAt | — | SalaryMax ≥ SalaryMin; MinExperience ≥ 0 |
| Resumes | Id | CandidateId | CandidateId, Headline, Summary, ExperienceYears, CreatedAt, UpdatedAt | CandidateId (1:1) | ExperienceYears ≥ 0 |
| ResumeDetails | ResumeId (=FK) | ResumeId | ResumeId | — (PK) | — |
| JobApplications | Id | CandidateId, VacancyId | CandidateId, VacancyId, Stage, AppliedAt, UpdatedAt | CandidateId + VacancyId | — |
| ResumeVacancyMatches | (ResumeId, VacancyId) | ResumeId, VacancyId | ResumeId, VacancyId, Status, MatchedAt | — (составной PK) | MatchScore 0…100 |
| Interviews | Id | ApplicationId | ApplicationId, ScheduledAt, Mode, Result | — | — |
| Interviewers | Id | — | FullName | Email | — |
| InterviewInterviewers | (InterviewId, InterviewerId) | InterviewId, InterviewerId | оба PK, Role | — (составной PK) | — |

Индексы: `Vacancies.Status`, `Resumes.ExperienceYears`, `JobApplications.Stage`, `JobApplications.AppliedAt`, `ResumeVacancyMatches.Status`, `Interviews.ScheduledAt`, уникальные — `Candidates.Email`, `Interviewers.Email`, `JobApplications(CandidateId, VacancyId)`, `Resumes.CandidateId`.

Таблицы Identity (AspNetUsers, AspNetRoles, …) создаются EF Core-миграцией и в скрипты не включены.

## Скрипты

```bash
# создание доменных таблиц
docker exec -i hrms-db mysql -uhrms -phrms hrms < database/01_create_tables.sql
# демо-данные
docker exec -i hrms-db mysql -uhrms -phrms hrms < database/02_seed_data.sql
```

Проверено на MySQL 8.4 в контейнере: create + seed проходят без ошибок; 6 кандидатов, 6 резюме, 5 вакансий, 7 откликов, 5 подборов, 5 собеседований, 7 участников.

## ER-диаграмма (текстовая)

```
Candidates 1───1 Resumes 1───1 ResumeDetails
     │ 1            │ 1
     │              │ N
     │              ▼
     │     ResumeVacancyMatches N───1 Vacancies
     │                              │ 1
     │ N                            │ N
     ▼                              ▼
JobApplications N───────────────────┘
     │ 1
     │ N
     ▼
Interviews N───1 JobApplications
     │ N
     ▼
InterviewInterviewers ←─ N ── Interviewers
```

## Миграция со старой схемы

Миграция `RestructureHiringPipeline` заменяет `Resumes.VacancyId/Status/SubmittedAt` на `Headline/CreatedAt/UpdatedAt/DesiredSalary`, делает `Resumes.CandidateId` уникальным, создаёт `JobApplications` и `ResumeVacancyMatches`, а `Interviews` переводит на `ApplicationId`.

Старые собеседования ссылались на резюме и вакансию напрямую; их нельзя однозначно преобразовать в отклик, поэтому миграция очищает `Interviews` перед пересборкой. Для учебной демо-базы это допустимо; при переносе реальных данных нужен отдельный скрипт сопоставления.

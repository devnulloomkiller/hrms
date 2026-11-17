# Задание и статус

Информационная система кадрового учёта: вакансии, кандидаты, резюме, отклики и собеседования.

| # | Пункт задания | Статус | Где |
|---|---------------|--------|-----|
| 0а | Схема БД: связи 1:1, 1:N, N:N, ключи, ограничения | ✅ | `docs/database-schema.md`, сущности в `backend/Hrms.Domain/Entities/` |
| 0б | Скрипты создания БД и таблиц | ✅ | `database/01_create_tables.sql` (MySQL 8.4) |
| 0в | Скрипты заполнения данными | ✅ | `database/02_seed_data.sql` (проверены на MySQL 8.4) |
| 1 | Создать новое решение ASP.NET | ✅ | `HRMS.slnx` в корне, слои в `backend/`, MVC в `frontend/Hrms.Web` |
| 2 | Организовать структуру папок для слоёв | ✅ | Domain/Application/Infrastructure/Web |
| 3 | Добавить пакеты NuGet | ✅ | EF Core (MySql.EntityFrameworkCore 11 preview), Design, Identity.EntityFrameworkCore |
| 4 | Создать модели (классы) для всех сущностей | ✅ | `backend/Hrms.Domain/Entities/`, `backend/Hrms.Domain/Enums/` |
| 5 | Создать `ApplicationDbContext` от `DbContext` | ✅ | `backend/Hrms.Infrastructure/Data/` (наследует `IdentityDbContext`) |
| 6 | Базовые CRUD-операции (реализовать и продемонстрировать) | ✅ | Vacancies/Candidates/Applications/Interviews: контроллеры + формы; резюме редактируется в карточке кандидата |
| 7 | Нетривиальные запросы, 3 шт. | ✅ | `IReportService`: истекающие дедлайны, профиль кандидата, сводка собеседований + воронка по вакансиям |
| 8 | Аутентификация и авторизация | ✅ | Identity (cookie), роли Admin/HR, seed-админ, [Authorize] на контроллерах |

Статус обновлять вместе с CHANGELOG при завершении каждого пункта.

## Доработка связей (2026-11-17)

| Что | Статус | Где |
|---|---|---|
| Кандидат связан с вакансиями через N:N-отклики с этапом воронки | ✅ | `JobApplication`, `JobApplicationsController`, `Views/Applications/` |
| Резюме привязано к кандидату 1:1 | ✅ | `Resumes.CandidateId` уникален, `Candidate.Resume`, общая форма кандидата |
| Резюме связано с вакансиями N:N с фильтрами и оценкой соответствия | ✅ | `ResumeVacancyMatch`, `ResumeService.FindResumesForVacancyAsync`, `Views/Vacancies/Match.cshtml` |
| Обратный подбор вакансий под резюме | ✅ | `ResumeService.FindVacanciesForResumeAsync`, `Views/Candidates/Match.cshtml` |
| Собеседования привязаны к отклику, а не к паре резюме+вакансия | ✅ | `Interview.ApplicationId`, `InterviewService` |
| Демо-данные создаются автоматически при пустой базе | ✅ | `DbInitializer.SeedDemoDataAsync` |
| Воронка откликов и фильтры в интерфейсе | ✅ | `Views/Applications/Index.cshtml`, фильтры в списках |

## Демо-доступ

- Админ: `admin@hrms.local` / `Admin123`
- Регистрация (`/Account/Register`) создаёт пользователя с ролью HR
- При первом старте на пустой базе создаются 6 кандидатов, 5 вакансий, 7 откликов, подборы и собеседования

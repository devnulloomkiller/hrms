# Задание и статус

Информационная система кадрового учёта: вакансии, резюме, собеседования.

| # | Пункт задания | Статус | Где |
|---|---------------|--------|-----|
| 0а | Схема БД: связи 1:1, 1:N, N:N, ключи, ограничения | ✅ | `docs/database-schema.md`, сущности в `backend/Hrms.Domain/Entities/` |
| 0б | Скрипты создания БД и таблиц | ✅ | `database/01_create_tables.sql` |
| 0в | Скрипты заполнения данными | ✅ | `database/02_seed_data.sql` (проверены на MySQL 8.4) |
| 1 | Создать новое решение ASP.NET | ✅ | `HRMS.slnx` в корне, слои в `backend/`, MVC в `frontend/Hrms.Web` |
| 2 | Организовать структуру папок для слоёв | ✅ | Domain/Application/Infrastructure/Web |
| 3 | Добавить пакеты NuGet | ✅ | EF Core (MySql.EntityFrameworkCore 11 preview), Design, Identity.EntityFrameworkCore |
| 4 | Создать модели (классы) для всех сущностей | ✅ | `backend/Hrms.Domain/Entities/`, `backend/Hrms.Domain/Enums/` |
| 5 | Создать `ApplicationDbContext` от `DbContext` | ✅ | `backend/Hrms.Infrastructure/Data/` (наследует `IdentityDbContext`) |
| 6 | Базовые CRUD-операции (реализовать и продемонстрировать) | ✅ | Vacancies/Candidates/Resumes/Interviews: контроллеры + представления, проверены curl'ом |
| 7 | Нетривиальные запросы, 3 шт. | ✅ | `IReportService`: истекающие дедлайны, профиль кандидата, сводка собеседований; страницы Reports |
| 8 | Аутентификация и авторизация | ✅ | Identity (cookie), роли Admin/HR, seed-админ, [Authorize] на контроллерах |

Статус обновлять вместе с CHANGELOG при завершении каждого пункта.

## Кандидаты в нетривиальные запросы — реализованы

1. Вакансии с истекающим сроком приёма резюме → `Reports/ExpiringVacancies`
2. Все резюме и собеседования кандидата → `Reports/CandidateProfile`
3. Количество собеседований по вакансиям за период (по результатам) → `Reports/InterviewSummary`

## Демо-доступ

- Админ: `admin@hrms.local` / `Admin123`
- Регистрация (`/Account/Register`) создаёт пользователя с ролью HR

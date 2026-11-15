# Implement reports and authentication per assignment

## Context and Problem Statement

Задание требует (пункты 7–8): три нетривиальных запроса к системе и аутентификацию с авторизацией. Схема БД уже содержит связи 1:1, 1:N и N:N (`docs/database-schema.md`), CRUD реализован. Нужно решить, где живут запросы, как строится авторизация и как разграничиваются роли, не усложняя учебный проект.

## Decision

**Нетривиальные запросы** оформляем отдельным сервисом `IReportService`/`ReportService` в `Hrms.Application` с проекциями в record-DTO (`Hrms.Application/DTOs`), без сырого SQL — так каждый запрос компилируем и тестируем единственным способом. Три запроса:

1. `GetVacanciesWithExpiringDeadlineAsync(days)` — открытые вакансии с дедлайном приёма резюме в ближайшие N дней + счётчик уникальных резюме (подсчёт через навигацию Vacancy→Interviews→ResumeId Distinct, потому что резюме без собеседований к вакансии связаны nullable-FK).
2. `GetCandidateProfileAsync(id)` — агрегированный профиль: все резюме кандидата + все собеседования с участниками N:N, одним запросом с вложенными проекциями.
3. `GetInterviewSummaryByVacancyAsync(from, to)` — собеседования по вакансиям за период с разбиением по результатам (условная агрегация `Count(predicate)` в группировке).

Пара DTO сортируется после `ToListAsync` — EF Core не транслирует `OrderBy` по полям спроецированного record-типа.

**Аутентификация/авторизация** — ASP.NET Core Identity поверх существующего `ApplicationDbContext` (`IdentityDbContext<ApplicationUser>`), cookie-сессии. Роли: `Admin` (полный доступ, управление ролями через `ManageController`) и `HR` (выдаётся при самостоятельной регистрации; работа с данными). Контроллеры данных — `[Authorize(Roles = "Admin,HR")]`, отчёты — `[Authorize]`. `DbInitializer.SeedRolesAndAdminAsync` создаёт роли и демо-админа `admin@hrms.local / Admin123` при старте.

Компромиссы: пароль демо-админа зашит в код (учебный проект; в production — конфиг/секреты). Регистрация открытая и сразу даёт HR (нет приглашений/подтверждения email) — достаточно для демонстрации, отмечено как non-goal.

Non-goals: подтверждение email, восстановление пароля, external login, JWT (задание — MVC-приложение, cookie достаточны).

## Alternatives Considered

- **Сырой SQL (`FromSqlInterpolated`) для запросов**: больше контроля над планом запроса, но теряется проверка типов на компиляции и переносимость между провайдерами; для трёх запросов на SQLite не оправдано.
- **Запросы прямо в контроллерах через DbContext**: быстрее, но нарушает послойность (Web не должен знать EF) и переиспользование.
- **JWT-аутентификация**: подходит для SPA/API, но задание — серверный MVC; cookie Identity даёт готовый жизненный цикл (login/logout/lockout) без единой строчки JS.
- **Политики (policies) вместо ролей**: гибче, но для двух фиксированных ролей `[Authorize(Roles = ...)]` проще и нагляднее для демонстрации.

## More Information

- Реализация: `src/Hrms.Application/Services/ReportService.cs`, `src/Hrms.Web/Controllers/ReportsController.cs`, `src/Hrms.Web/Controllers/AccountController.cs`, `src/Hrms.Infrastructure/DbInitializer.cs`
- Связанный ADR: [Build on ASP.NET Core with EF Core](2026-11-15-build-on-aspnet-core-with-ef-core.md)
- Пересмотреть, если появится API-клиент (тогда JWT) или больше ролей/прав (тогда policies + claims).

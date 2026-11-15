# HRMS

Информационная система кадрового учёта: вакансии, резюме, собеседования. Учебный проект.

## Документация для агентов (читать в начале сессии, по порядку)

1. `CHANGELOG.md` — журнал: что сделано, что запланировано. Обновлять при завершении работы.
2. `docs/requirements.md` — задание и статус по пунктам. Обновлять вместе с CHANGELOG.
3. `contributing/decisions/README.md` — индекс ADR.
4. Правила работы: `AGENTS.md` (adr-skill, antislop), `~/.config/zed/AGENTS.md` (глобальные).

## Стек

- ASP.NET Core 11, MVC
- Entity Framework Core 11 + SQLite (разработка/демонстрация)
- ASP.NET Core Identity (аутентификация, роли Admin/HR)

Решение зафиксировано в ADR: `contributing/decisions/2026-11-15-build-on-aspnet-core-with-ef-core.md`

## Структура

```
HRMS.slnx
src/
  Hrms.Domain/         # сущности: Candidate, Resume, Vacancy, Interview (+ enum'ы)
  Hrms.Application/    # сервисы и нетривиальные запросы (DTO, IQueryable)
  Hrms.Infrastructure/ # ApplicationDbContext, конфигурация EF Core, Identity-хранилище
  Hrms.Web/            # MVC: контроллеры, представления, авторизация
contributing/
  decisions/           # ADR + индекс README.md
docs/
  requirements.md      # задание и статус выполнения
CHANGELOG.md           # журнал изменений
```

Зависимости слоёв: `Web → Infrastructure → Application → Domain`.

## Демо-доступ

- Админ: `admin@hrms.local` / `Admin123` (создаётся автоматически при старте)
- Регистрация (`/Account/Register`) — пользователь с ролью HR
- Демо-данные: после первого старта `sqlite3 src/Hrms.Web/hrms.db < database/02_seed_data.sql`

## Сборка и запуск

```bash
dotnet build
dotnet run --project src/Hrms.Web
```

База: SQLite-файл, создаётся миграциями (команда миграций появится после первого `dotnet ef` — см. CHANGELOG).

## Документация для агентов

См. раздел «Документация для агентов» выше: порядок чтения — CHANGELOG → requirements → индекс ADR → правила AGENTS.md.

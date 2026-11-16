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
HRMS.slnx              # решение в корне репозитория
backend/               # серверные слои
  Hrms.Domain/         # Entities/, Enums/ — сущности и перечисления
  Hrms.Application/    # Interfaces/, Services/, DTOs/ — сервисы и запросы (DTO, IQueryable)
  Hrms.Infrastructure/ # Data/, Migrations/ — ApplicationDbContext, EF Core, Identity-хранилище
frontend/              # презентационный слой
  Hrms.Web/            # Controllers/, ViewModels/, Extensions/, Views/, wwwroot/
contributing/
  decisions/           # ADR + индекс README.md
docs/
  requirements.md      # задание и статус выполнения
database/              # SQL-скрипты создания и заполнения БД
CHANGELOG.md           # журнал изменений
```

Зависимости слоёв: `frontend/Hrms.Web → backend/Hrms.Infrastructure → backend/Hrms.Application → backend/Hrms.Domain`.

## Демо-доступ

- Админ: `admin@hrms.local` / `Admin123` (создаётся автоматически при старте)
- Регистрация (`/Account/Register`) — пользователь с ролью HR
- Демо-данные: после первого старта `sqlite3 frontend/Hrms.Web/hrms.db < database/02_seed_data.sql`

## Сборка и запуск

```bash
dotnet build
dotnet run --project frontend/Hrms.Web
```

База: SQLite-файл, создаётся миграциями (команда миграций появится после первого `dotnet ef` — см. CHANGELOG).

## Документация для агентов

См. раздел «Документация для агентов» выше: порядок чтения — CHANGELOG → requirements → индекс ADR → правила AGENTS.md.

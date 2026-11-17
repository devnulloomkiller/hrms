# HRMS

Информационная система кадрового учёта: вакансии, кандидаты, резюме, отклики и собеседования. Учебный проект.

## Документация для агентов (читать в начале сессии, по порядку)

1. `CHANGELOG.md` — журнал: что сделано, что запланировано. Обновлять при завершении работы.
2. `docs/requirements.md` — задание и статус по пунктам. Обновлять вместе с CHANGELOG.
3. `contributing/decisions/README.md` — индекс ADR.
4. Правила работы: `AGENTS.md` (adr-skill, antislop), `~/.config/zed/AGENTS.md` (глобальные).

## Стек

- ASP.NET Core 11, MVC
- Entity Framework Core 11 + MySQL 8.4 (Docker, разработка/демонстрация)
- ASP.NET Core Identity (аутентификация, роли Admin/HR)

Решение зафиксировано в ADR: `contributing/decisions/2026-11-15-build-on-aspnet-core-with-ef-core.md`

## Как устроен найм

- **Кандидат** — контакты и одно резюме (связь 1:1). Резюме редактируется в карточке кандидата.
- **Отклик** — связь N:N «кандидат ↔ вакансия» с этапом воронки: новый отклик, скрининг, собеседование, оффер, найм, отказ или отзыв. На один отклик можно назначить несколько собеседований.
- **Подбор** — связь N:N «резюме ↔ вакансия» с оценкой соответствия и статусом (предложен, шортлист, не подходит). Подбор не равен отклику: HR может сначала собрать шортлист, а кандидат откликнется позже.
- Оценка соответствия: опыт (40), совпадение обязательных навыков (40), попадание желаемой зарплаты в вилку (20). Фильтры подбора: активность вакансии, минимальный опыт, навык, верхняя граница зарплаты, скрытие уже связанных.

Подробности схемы: `docs/database-schema.md`.

## Структура

```
HRMS.slnx              # решение в корне репозитория
backend/               # серверные слои
  Hrms.Domain/         # Entities/, Enums/ — сущности и перечисления
  Hrms.Application/    # Interfaces/, Services/, DTOs/, Filters/ — сервисы, фильтры, запросы
  Hrms.Infrastructure/ # Data/, Migrations/, DbInitializer — EF Core, Identity, демо-данные
frontend/              # презентационный слой
  Hrms.Web/            # Controllers/, ViewModels/, Views/, wwwroot/
contributing/
  decisions/           # ADR + индекс README.md
docs/
  requirements.md      # задание и статус выполнения
  database-schema.md   # связи, ключи, ER-диаграмма
database/              # SQL-скрипты создания и заполнения БД
CHANGELOG.md           # журнал изменений
```

Зависимости слоёв: `frontend/Hrms.Web → backend/Hrms.Infrastructure → backend/Hrms.Application → backend/Hrms.Domain`.

## Демо-доступ

- Админ: `admin@hrms.local` / `Admin123` (создаётся автоматически при старте)
- Регистрация (`/Account/Register`) — пользователь с ролью HR
- Демо-данные создаются автоматически при первом старте на пустой базе: 6 кандидатов с резюме, 5 вакансий, 7 откликов, подборы, собеседования и интервьюеры. Ручной импорт `database/02_seed_data.sql` нужен только для демонстрации SQL-скриптов на отдельной базе.

## Сборка и запуск

```bash
docker compose up -d
dotnet build
dotnet run --project frontend/Hrms.Web
```

Приложение: `http://localhost:5264` (профиль `http`). База: MySQL 8.4 в Docker (контейнер `hrms-db`); схема создаётся миграциями при первом старте.

## SQL-скрипты

```bash
docker exec -i hrms-db mysql -uhrms -phrms hrms < database/01_create_tables.sql
docker exec -i hrms-db mysql -uhrms -phrms hrms < database/02_seed_data.sql
```

Скрипты пересоздают доменные таблицы и наполняют их демо-данными; Identity-таблицы не затрагиваются. На пустой базе приложение делает то же самое автоматически.

## Документация для агентов

См. раздел «Документация для агентов» выше: порядок чтения — CHANGELOG → requirements → индекс ADR → правила AGENTS.md.

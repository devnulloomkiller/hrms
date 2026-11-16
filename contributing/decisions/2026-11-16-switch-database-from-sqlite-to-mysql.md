# Switch development database from SQLite to MySQL

## Context and Problem Statement

Приложение разрабатывалось на SQLite (`Data Source=hrms.db`), выбор зафиксирован в [Build on ASP.NET Core with EF Core](2026-11-15-build-on-aspnet-core-with-ef-core.md). Появилось требование работать с MySQL, развёрнутой в Docker; PostgreSQL исключён заказчиком. Провайдер должен быть совместим с EF Core 11, на котором собран весь стек: у Pomelo.EntityFrameworkCore.MySql сборок под EF Core 11 нет (максимум 9.0.0), у Oracle MySql.EntityFrameworkCore существует 11.0.0-preview.

## Decision

- MySQL 8.4 (LTS) в Docker Compose: `docker-compose.yml` в корне — контейнер `hrms-db`, база и пользователь `hrms`, том `hrms-db-data`, healthcheck `mysqladmin ping`.
- Провайдер — Oracle `MySql.EntityFrameworkCore 11.0.0-preview`, единственный с сборками под EF Core 11. Подключение — `UseMySQL(connectionString)` в `AddInfrastructure`; строка в `appsettings.json` (`ConnectionStrings:Default`), fallback в `Program.cs`.
- Миграции сгенерированы заново (SQLite-миграции удалены): `DateOnly` → `date`, `DateTime` → `datetime(6)`, чек-констрейнт оклада переписан с ANSI-кавычек на MySQL-бэктики (иначе sql_mode по умолчанию трактует `"..."` как строковый литерал).
- SQL-скрипты `database/*.sql` переписаны под диалект MySQL (`AUTO_INCREMENT`, `VARCHAR`/`DATE`/`DATETIME`, `CHAR_LENGTH`) и проверены на чистой scratch-БД в контейнере.
- Принятые компромиссы: провайдер в preview-статусе (стабильная ветка 10.0.12 потребовала бы даунгрейда всего стека до EF Core 10); демонстрационные креды в `docker-compose.yml` и `appsettings.json` (учебный проект); MySQL-креды совпадают в compose, appsettings и скриптах.
- Non-goals: прод-развертывание, вынос секретов из репозитория, смена провайдера/СУБД в обозримом будущем.

## Alternatives Considered

- **Pomelo.EntityFrameworkCore.MySql**: де-факто стандарт для EF Core + MySQL с высокими оценками сообщества, но под EF Core 11 сборок нет; стабильный 9.0.0 тянет EF Core 9 и конфликтует с остальными 11-ми пакетами. Отклонено.
- **MySql.EntityFrameworkCore 10.0.12 (стабильный)**: потребовал бы даунгрейда всех EF-пакетов до 10.0 — большой churn слоёв ради стабильности одного пакета в учебном проекте. Отклонено.
- **PostgreSQL + Npgsql**: зрелый провайдер под актуальный EF Core существует, но PostgreSQL исключён заказчиком. Отклонено.
- **Оставить SQLite**: не отвечает требованию работать с MySQL из Docker. Отклонено.

## More Information

- Частично заменяет [Build on ASP.NET Core with EF Core](2026-11-15-build-on-aspnet-core-with-ef-core.md): слои, зависимости и выбор ASP.NET Core не меняются — заменена только СУБД разработки и её провайдер.
- Запуск: `docker compose up -d`, затем `dotnet run --project frontend/Hrms.Web`; схема создаётся миграцией при старте, роли и демо-админ засеиваются `DbInitializer`.
- Пересмотреть решение, когда Pomelo выпустит сборку под актуальный EF Core либо Oracle объявит стабильный 11.x.

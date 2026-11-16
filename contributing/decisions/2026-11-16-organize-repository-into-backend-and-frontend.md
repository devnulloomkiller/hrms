# Organize repository into backend and frontend folders

## Context and Problem Statement

Проект — монолитное server-rendered MVC-приложение: Razor-представления и статика физически живут внутри `Hrms.Web` вместе с контроллерами. Все четыре проекта лежали плоско в `src/`, из-за чего с первого взгляда не видно, где серверная логика, а где UI. Внутри проектов накопилась неоднородность: enum'ы объявлялись прямо в файлах сущностей (`Hrms.Domain/Entities`), view-модели и UI-хелперы — в одной папке `Hrms.Web/Models`. Реструктуризация выполняется перед сдачей задания, поэтому важны скорость и нулевой риск для работающего приложения.

## Decision

- Решение `HRMS.slnx` переносится в корень репозитория; проекты группируются в две solution-папки.
- `backend/` — серверные слои: `Hrms.Domain` (`Entities/`, `Enums/`), `Hrms.Application` (`Interfaces/`, `Services/`, `DTOs/`), `Hrms.Infrastructure` (`Data/`, `Migrations/`).
- `frontend/` — презентационный слой: `Hrms.Web` целиком (`Controllers/`, `ViewModels/`, `Extensions/`, `Views/`, `wwwroot/`). Контроллеры остаются в Web: в Razor-MVC они часть презентации (маршрутизация, валидация ввода, выбор представления), отдельного API нет.
- Внутренняя чистка: enum'ы вынесены в `Hrms.Domain/Enums` (namespace `Hrms.Domain.Enums`), view-модели — в `frontend/Hrms.Web/ViewModels` (`Hrms.Web.ViewModels`), UI-хелперы — в `Extensions/` (`Hrms.Web.Extensions`); DTO остаются в `Hrms.Application/DTOs`.
- Принятый компромисс: «frontend» здесь означает презентационный слой MVC, а не SPA — в папке лежит и C#-код контроллеров.
- Non-goals: выделение UI в отдельный Razor Class Library, внедрение SPA-фронтенда, изменение состава слоёв и зависимостей `Web → Infrastructure → Application → Domain`.

## Alternatives Considered

- **Razor Class Library (`frontend/Hrms.UI` с Views/wwwroot, в `backend/Hrms.Web` только контроллеры и хост)**: честнее разделяет UI и бэкенд, но добавляет пятый проект, усложняет сборку и отладку и повышает риск перед демонстрацией. Отклонено как избыточное для текущего размера приложения.
- **Оставить всё решение в `backend/src` без папки frontend**: ноль правок и честно для монолита, но граница презентационного слоя остаётся скрытой, а каталог верхнего уровня не отвечает на вопрос «где frontend». Отклонено.
- **Оставить enum'ы внутри файлов сущностей**: ноль правок, но namespace не соответствует папке, а типы статусов замусоривают файлы сущностей. Отклонено.

## More Information

- Связанный ADR: [Build on ASP.NET Core with EF Core](2026-11-15-build-on-aspnet-core-with-ef-core.md) — слои, зависимости и стек не меняются; переезд затрагивает только физическое расположение проектов и группировку типов. Пути `src/...` в более старых ADR — исторические; актуальная структура описана в README.
- Пересмотреть, если появится отдельный SPA-фронтенд или API-слой: тогда `frontend/` получит собственный проект, а `Hrms.Web` разделится на хост API и UI-библиотеку.

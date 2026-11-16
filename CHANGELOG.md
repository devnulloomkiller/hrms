# Changelog

Формат: дата — что сделано. Новые записи сверху. Читать в начале каждой сессии, чтобы восстановить контекст проекта.

## [Unreleased]

- Скиллы вынесены из репозитория: `adr-skill` → глобальный `~/.agents/skills/adr-skill` (+ симлинк `~/.claude/skills/adr-skill` по общему паттерну), дизайн-скиллы (ui-ux-pro-max, design, design-system и др.) синхронизированы со свежими проектными копиями в `~/.agents/skills/`, antislop остался в `~/.config/zed/` (проектные копии были идентичны глобальным). Из репо удалены `.agents/`, `skills/`, `antislop.md`, `skills-lock.json`, локальные каталоги инструментов `.zcode/`, `.serena/`; пути в AGENTS.md переписаны на глобальные, из `.gitignore` убраны устаревшие записи agent-state
- Рефакторинг структуры репозитория: `HRMS.slnx` в корне, серверные слои в `backend/` (Domain, Application, Infrastructure), MVC-приложение в `frontend/Hrms.Web`. ADR: `contributing/decisions/2026-11-16-organize-repository-into-backend-and-frontend.md`
- Порядок внутри проектов: enum'ы — в `backend/Hrms.Domain/Enums/` (namespace `Hrms.Domain.Enums`), view-модели — в `frontend/Hrms.Web/ViewModels/` (`Hrms.Web.ViewModels`), PillExtensions — в `frontend/Hrms.Web/Extensions/` (`Hrms.Web.Extensions`), DTO — в `backend/Hrms.Application/DTOs/` (как и было). Ссылки на статусы в Razor переписаны на новый namespace
- Проверено: `dotnet build` — 0 ошибок/0 предупреждений; smoke curl: `/` и `/Account/Login` → 200, `/Vacancies` анонимом → 302 на Login, `/css/site.css` → 200; БД `frontend/Hrms.Web/hrms.db` переехала вместе с проектом
- Палитра интерфейса переведена в монохром: чёрный, белый и нейтральные оттенки серого для светлой и тёмной тем, включая статусы, кнопки, дедлайны и боковую навигацию.
- Полный SaaS-редизайн: боковая навигация с группами, Plus Jakarta Sans, светлая и тёмная темы, обновлённые таблицы, формы, отчёты, состояния и мобильные карточки.
- Новый дашборд на главной для авторизованных пользователей: реальные счётчики вакансий, кандидатов, резюме и собеседований, ближайшие дедлайны, последние отклики и быстрые действия. Данные анонимам не показываются.
- Проверено: `dotnet build`; Playwright на 1440/1024/768/375/320 px, обе темы, меню и возврат фокуса, фильтры отчётов, создание/изменение/удаление тестового кандидата. Метрики дашборда сверены с SQLite, ошибок браузерной консоли нет.
- Исправлена сборка `Hrms.Web`: полное имя `VacancyStatus` в `Views/Vacancies/Index.cshtml`.

Планируется:
- Финальная зачистка: шаблонные Home/Privacy при необходимости, README-скриншоты
- Опционально: seed бизнес-данных при старте (сейчас вручную: `sqlite3 frontend/Hrms.Web/hrms.db < database/02_seed_data.sql` после первого запуска)

## 2026-11-15 (5)

### Нетривиальные запросы (3 шт.) + аутентификация/авторизация

**Запросы** (`Hrms.Application`: `IReportService`/`ReportService`, DTO в `Hrms.Application/DTOs/ReportDtos.cs`):
1. `GetVacanciesWithExpiringDeadlineAsync(days)` — вакансии с дедлайном в ближайшие N дней + счётчик уникальных резюме (Distinct по ResumeId через навигацию Interviews)
2. `GetCandidateProfileAsync(id)` — профиль кандидата одним запросом: резюме + собеседования + участники N:N (вложенные проекции)
3. `GetInterviewSummaryByVacancyAsync(from,to)` — собеседования по вакансиям за период, разбиение по результатам (условный Count)
- Страницы: `ReportsController` + 3 представления (ExpiringVacancies с подсветкой ≤3 дней red / ≤7 yellow; CandidateProfile карточками; InterviewSummary с конверсией)
- Нюанс EF: OrderBy по полю record-DTO не транслируется — сортировка после ToListAsync (два места в ReportService)

**Identity**:
- `Program.cs`: AddIdentity + EntityFrameworkStores, cookie (LoginPath /Account/Login, AccessDeniedPath), пароль ≥6, уникальный email
- `DbInitializer.SeedRolesAndAdminAsync`: роли Admin/HR + демо-админ `admin@hrms.local / Admin123` при старте
- `AccountController`: Login/Logout/Register (выдаёт роль HR)/AccessDenied; `ManageController` [Authorize]: список пользователей, ToggleAdmin [Authorize(Roles=Admin)]
- Контроллеры данных: `[Authorize(Roles = "Admin,HR")]`, Reports: `[Authorize]`; аноним → 302 на Login с ReturnUrl
- `_Layout.cshtml`: навигация по разделам, имя пользователя + Logout / «Войти», lang=ru

**Проверено (curl, http://localhost:5264)**:
- Аноним: /Vacancies и /Reports → 302 на Login; логин admin → 302, далее 200
- Отчёт 1 (days=10): HR Manager (red, дедлайн 18.11), Senior .NET Dev (yellow, 20.11)
- Отчёт 2: Ivanov — 2 резюме, 3 собеседования, участники на месте
- Отчёт 3: Senior .NET — 4 собеседования, конверсия 75%
- Регистрация hr@hrms.local → роль HR → данные доступны; Manage: admin видит ToggleAdmin

**ADR**: `2026-11-15-implement-reports-and-authentication.md` (запросы через проекции в Application; Identity cookie + роли; альтернативы: сырой SQL, JWT, policies). Индекс обновлён.

## 2026-11-15 (6)

### UI: тёмная тема по design system из ui-ux-pro-max

- Design system сгенерирован скиллом и сохранён: `design-system/hrms/MASTER.md` (источник правды для UI): glassmorphism-dashboard профиль, тёмный slate (#0F172A фон, #22C55E акцент), Fira Sans/Code, backdrop-blur карточки
- `site.css` переписан на CSS-переменные из MASTER: sidebar-лейаут (сайдбар 232px на десктопе, топбар на мобиле), стеклянные карточки, таблицы в карточках с overflow-x, статусы-пиллы (цвет + русский текст), focus-visible, prefers-reduced-motion
- `_Layout.cshtml`: сайдбар с SVG-иконками (без эмодзи), активный пункт по контроллеру, пользователь + logout внизу
- Index-страницы всех сущностей: таблицы в карточках, пустые состояния, кнопки-действия, пиллы статусов (`PillExtensions`: RU-названия + Pill())
- Отчёты: подсветка строк row-danger/warning, карточки-счётчики в профиле кандидата
- Логин: центрированная карточка; Home: карточки разделов
- Попутно: build 0 ошибок/0 предупреждений

### Git

- Репозиторий инициализирован, история разбита на 13 коммитов по слоям/зонам (docs → ADR → database → domain → infrastructure → application → web controllers → UI → skills)
- Remote: `git@github.com:devnulloomkiller/hrms.git` (origin). Пуш — вручную
- `.gitignore`: bin/obj, *.db, IDE, agent state (.serena, .insect), node_modules

## 2026-11-15 (4)

### CRUD: все сущности

- `ICandidateService`/`CandidateService`, `IResumeService`/`ResumeService`, `IInterviewService`/`InterviewService` (+регистрация в DI). InterviewService: includes через `.Include().ThenInclude()` (не через Query-параметры), Update пересобирает участников N:N
- Контроллеры: `CandidatesController`, `ResumesController` (селекторы кандидат/вакансия через ViewBag), `InterviewsController` (форма принимает resumeId/vacancyId/scheduledAt/mode/result/notes/interviewerIds[]/roles[] — участники N:N чекбоксами + роль текстом)
- Представления: по 4 на сущность (Index/Details/Form/Delete), Form общая для Create+Edit. Vacancies остаются на отдельных Create/Edit из ранней итерации
- Исправлено по ходу: NRE в Razor-формах (View("Form") без модели → передаются new-экземпляры с required-полями = ""), дубль isEdit, ThenInclude через параметры Query не работает
- Smoke-тест: все Index/Create отдают 200, POST кандидата 302 → запись видна в списке и Details. БД: свежая + `database/02_seed_data.sql` (Identity-таблицы уже созданы миграцией при первом старте — поэтому seed после первого запуска)
- Известный шум в логе: EF-миграция `ExtendedSchema_...` пишет warning о non-transactional migration (SQLite + rebuild) — не блокирует, при чистой БД не воспроизводится

## 2026-11-15 (3)

### Схема БД: полный набор связей + SQL-скрипты

- Домен расширен для задания «анализ и разработка схемы БД»:
  - `ResumeDetails` — 1:1 с Resume (PK=FK ResumeId, cascade): Education, Skills, FilePath (перенесены из Resume)
  - `Interviewer` — справочник интервьюеров (FullName, Email уникальный, Position)
  - `InterviewInterviewer` — junction N:N (составной PK InterviewId+InterviewerId, атрибут Role: Host/Technical/HR/Observer)
  - `Interview.InterviewerName` удалён, заменён связью N:N
- `ApplicationDbContext`: новые DbSet'ы и fluent-конфигурация (1:1 HasKey(ResumeId)+WithOne, N:N HasKey составной, уникальный индекс Email у Interviewer)
- Миграция `ExtendedSchema_ResumeDetails_Interviewers`
- SQL-скрипты (SQLite, проверены на реальной БД):
  - `database/01_create_tables.sql` — DROP+CREATE всех таблиц, PK/FK/CHECK/UNIQUE/индексы
  - `database/02_seed_data.sql` — демо-данные: 5 кандидатов, 5 вакансий, 9 резюме, 5 деталей, 8 собеседований, 5 интервьюеров, 12 участников; JOIN через N:N проверен
- `docs/database-schema.md` — таблица связей (1:1, 1:N, N:N), сводка ключей/ограничений, текстовая ER-диаграмма, инструкция запуска скриптов
- Проверка: sqlite3 create+seed без ошибок; выборка через junction корректна; dotnet build 0 ошибок

## 2026-11-15 (2)

### CRUD: вакансии (полный цикл, проверен curl'ом)

- `Hrms.Application`: `IHrmsRepository<TEntity>` (Query с includes, GetById, Add, Remove, SaveChanges), `IVacancyService` + `VacancyService` (CRUD, сортировка по отделу/названию). Пакет `Microsoft.EntityFrameworkCore` добавлен в Application (для IQueryable-расширений — так задумано ADR: запросы в Application)
- `Hrms.Infrastructure`: `EfRepository<TEntity>`, `DependencyInjection.AddInfrastructure(connectionString)` — DbContext, репозиторий, сервис; `FrameworkReference Microsoft.AspNetCore.App` в Infrastructure-проекте
- `Hrms.Web`: `VacanciesController` (Index/Details/Create/Edit/Delete, [ValidateAntiForgeryToken]), представления Views/Vacancies/*.cshtml (5 шт., Bootstrap, валидация), `Program.cs`: AddInfrastructure + `db.Database.Migrate()` при старте
- `Microsoft.EntityFrameworkCore.Design` добавлен в Hrms.Web (нужен dotnet-ef)
- Миграция `InitialCreate` в `Hrms.Infrastructure/Migrations/` (создана через dotnet-ef 11, tool установлен глобально, PATH: `~/.dotnet/tools`)
- Проверено: build 0 ошибок; запуск на http://localhost:5264; CREATE (302) → READ (в списке) → UPDATE (данные изменились) → DELETE (список пуст). Antiforgery работает (без токена — 400)
- Баги по пути: include скалярного свойства в Query() (исправлено на OrderBy); MigrationsNotFound из-за несобранного билда (после build — ок)
- Строка подключения: `Data Source=hrms.db` (default в Program.cs, appsettings пока не содержит секцию ConnectionStrings)

## 2026-11-15

### Каркас решения

- Решение `HRMS.slnx` (новый формат), четыре проекта в `src/`:
  - `Hrms.Domain` (net11.0) — без зависимостей
  - `Hrms.Application` (net11.0) — ссылка на Domain
  - `Hrms.Infrastructure` (net11.0) — ссылка на Application
  - `Hrms.Web` (net11.0, MVC) — ссылка на Infrastructure
- Шаблонные `Class1.cs` удалены.

### NuGet (все в Hrms.Infrastructure, 11.0.0)

- `Microsoft.EntityFrameworkCore.Sqlite`
- `Microsoft.EntityFrameworkCore.Design`
- `Microsoft.AspNetCore.Identity.EntityFrameworkCore`

### Модели (`Hrms.Domain/Entities/`)

- `Candidate`: LastName, FirstName, Email (уникальный), Phone; коллекция Resume
- `Resume`: CandidateId (cascade), VacancyId (set null), Summary, ExperienceYears, Education, Skills, FilePath, SubmittedAt, Status; коллекция Interview
- `Vacancy`: Title, Department, Description, SalaryMin/SalaryMax (check-констрейнт SalaryMax >= SalaryMin), Status, PublishedAt, AcceptingResumesUntil; коллекция Interview
- `Interview`: ResumeId (cascade), VacancyId (restrict), InterviewerName, ScheduledAt, Mode, Result, Notes
- Enum'ы: `VacancyStatus` (Draft/Open/Paused/Closed), `ResumeStatus` (New/InReview/Rejected/Accepted), `InterviewMode` (Onsite/Phone/Video), `InterviewResult` (Scheduled/Passed/Failed/Cancelled)

### DbContext (`Hrms.Infrastructure/Data/`)

- `ApplicationDbContext : IdentityDbContext<ApplicationUser>`: DbSet'ы Vacancies/Candidates/Resumes/Interviews, fluent-конфигурация (макс. длины, точность decimal, индексы Status/ScheduledAt/Email, каскады, check-констрейнт)
- `ApplicationUser : IdentityUser` + DisplayName

### Решения и процесс

- ADR-практика: `contributing/decisions/` + bootstrap-ADR + индекс
- ADR `2026-11-15-build-on-aspnet-core-with-ef-core.md`: ASP.NET Core + EF Core + SQLite + Identity, слои. Отступление: планировался net8.0, на машине только SDK 11 — проекты на net11.0 (заметка в ADR есть)
- Установлены skills: adr-skill (проект), antislop + caveman (глобально, `~/.config/zed/`)
- `dotnet build` — 0 ошибок, 0 предупреждений

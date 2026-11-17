-- HRMS: демо-данные (MySQL 8.4)
-- Выполнять после 01_create_tables.sql. Скрипт идемпотентен: сначала очищает доменные таблицы.

SET NAMES utf8mb4;
SET FOREIGN_KEY_CHECKS = 0;
DELETE FROM `InterviewInterviewers`;
DELETE FROM `Interviews`;
DELETE FROM `ResumeVacancyMatches`;
DELETE FROM `JobApplications`;
DELETE FROM `ResumeDetails`;
DELETE FROM `Resumes`;
DELETE FROM `Interviewers`;
DELETE FROM `Vacancies`;
DELETE FROM `Candidates`;
SET FOREIGN_KEY_CHECKS = 1;

INSERT INTO `Candidates` (`Id`, `LastName`, `FirstName`, `Email`, `Phone`) VALUES
(1, 'Иванова', 'Анна', 'anna.ivanova@example.com', '+375 29 111-22-33'),
(2, 'Петров', 'Максим', 'maksim.petrov@example.com', '+375 29 222-33-44'),
(3, 'Смирнова', 'Ольга', 'olga.smirnova@example.com', '+375 29 333-44-55'),
(4, 'Козлов', 'Дмитрий', 'dmitry.kozlov@example.com', '+375 29 444-55-66'),
(5, 'Соколова', 'Елена', 'elena.sokolova@example.com', '+375 29 555-66-77'),
(6, 'Лебедев', 'Игорь', 'igor.lebedev@example.com', '+375 29 666-77-88');

INSERT INTO `Vacancies` (`Id`, `Title`, `Department`, `Description`, `SalaryMin`, `SalaryMax`, `MinExperience`, `RequiredSkills`, `Status`, `PublishedAt`, `AcceptingResumesUntil`) VALUES
(1, 'Senior .NET Developer', 'Разработка', 'Разработка платформы внутренних сервисов, код-ревью и наставничество.', 280000, 420000, 5, 'C#, ASP.NET Core, EF Core, SQL', 1, DATE_SUB(CURDATE(), INTERVAL 14 DAY), DATE_ADD(CURDATE(), INTERVAL 12 DAY)),
(2, 'HR Generalist', 'HR', 'Полный цикл подбора на технические и продуктовые роли.', 120000, 160000, 3, 'Подбор, HR-аналитика, интервью', 1, DATE_SUB(CURDATE(), INTERVAL 10 DAY), DATE_ADD(CURDATE(), INTERVAL 5 DAY)),
(3, 'Product Designer', 'Продукт', 'Дизайн B2B-интерфейсов и развитие дизайн-системы.', 180000, 250000, 4, 'Figma, UX research, дизайн-системы', 1, DATE_SUB(CURDATE(), INTERVAL 7 DAY), DATE_ADD(CURDATE(), INTERVAL 20 DAY)),
(4, 'Data Analyst', 'Аналитика', 'Продуктовая аналитика и отчётность для команд.', 160000, 220000, 2, 'SQL, Python, Power BI', 2, DATE_SUB(CURDATE(), INTERVAL 20 DAY), DATE_ADD(CURDATE(), INTERVAL 30 DAY)),
(5, 'QA Engineer', 'Разработка', 'Тестовая стратегия, API- и UI-автотесты.', 130000, 180000, 3, 'Playwright, Selenium, API testing', 3, DATE_SUB(CURDATE(), INTERVAL 40 DAY), DATE_SUB(CURDATE(), INTERVAL 5 DAY));

INSERT INTO `Resumes` (`Id`, `CandidateId`, `Headline`, `Summary`, `ExperienceYears`, `DesiredSalary`, `CreatedAt`, `UpdatedAt`) VALUES
(1, 1, 'Senior .NET Developer', 'Веду разработку внутренних сервисов на .NET, отвечаю за архитектуру и код-ревью.', 7, 380000, DATE_SUB(NOW(), INTERVAL 30 DAY), NOW()),
(2, 2, '.NET Developer', 'Разрабатываю бизнес-приложения на ASP.NET Core, есть опыт с React и CI.', 4, 280000, DATE_SUB(NOW(), INTERVAL 25 DAY), NOW()),
(3, 3, 'HR Generalist', 'Веду полный цикл подбора: от брифа до оффера, выстраиваю процессы адаптации.', 5, 150000, DATE_SUB(NOW(), INTERVAL 20 DAY), NOW()),
(4, 4, 'Product Designer', 'Проектирую интерфейсы B2B-продуктов, развиваю дизайн-систему и исследую пользователей.', 6, 230000, DATE_SUB(NOW(), INTERVAL 18 DAY), NOW()),
(5, 5, 'Data Analyst', 'Собираю продуктовые метрики, строю отчёты и проверяю гипотезы на данных.', 3, 190000, DATE_SUB(NOW(), INTERVAL 12 DAY), NOW()),
(6, 6, 'QA Engineer', 'Отвечаю за тестовую стратегию и автотесты API и UI.', 8, 170000, DATE_SUB(NOW(), INTERVAL 10 DAY), NOW());

INSERT INTO `ResumeDetails` (`ResumeId`, `Education`, `Skills`, `FilePath`) VALUES
(1, 'БГУ, прикладная математика, 2017', 'C#, ASP.NET Core, EF Core, SQL, Docker', NULL),
(2, 'БГУИР, программная инженерия, 2020', 'C#, ASP.NET Core, React, SQL', NULL),
(3, 'БГЭУ, управление персоналом, 2019', 'Подбор, HR-аналитика, интервью, адаптация', NULL),
(4, 'БГАИ, дизайн, 2018', 'Figma, UX research, дизайн-системы, прототипирование', NULL),
(5, 'БГУ, экономика, 2021', 'SQL, Python, Power BI, A/B-тесты', NULL),
(6, 'БНТУ, информационные системы, 2016', 'Playwright, Selenium, API testing, Postman', NULL);

INSERT INTO `JobApplications` (`Id`, `CandidateId`, `VacancyId`, `Stage`, `AppliedAt`, `UpdatedAt`, `Source`, `Notes`) VALUES
(1, 1, 1, 2, DATE_SUB(NOW(), INTERVAL 6 DAY), NOW(), 'Карьерный сайт', 'Сильное портфолио, назначено техническое интервью.'),
(2, 2, 1, 1, DATE_SUB(NOW(), INTERVAL 4 DAY), NOW(), 'LinkedIn', NULL),
(3, 3, 2, 3, DATE_SUB(NOW(), INTERVAL 8 DAY), NOW(), 'Рекомендация', NULL),
(4, 4, 3, 2, DATE_SUB(NOW(), INTERVAL 3 DAY), NOW(), 'Карьерный сайт', NULL),
(5, 5, 4, 0, DATE_SUB(NOW(), INTERVAL 2 DAY), NOW(), 'hh.ru', NULL),
(6, 6, 5, 5, DATE_SUB(NOW(), INTERVAL 20 DAY), NOW(), 'hh.ru', 'Не совпал формат работы.'),
(7, 2, 5, 6, DATE_SUB(NOW(), INTERVAL 18 DAY), NOW(), 'LinkedIn', NULL);

INSERT INTO `ResumeVacancyMatches` (`ResumeId`, `VacancyId`, `Status`, `MatchScore`, `MatchedAt`, `Comment`) VALUES
(1, 3, 0, 60, DATE_SUB(NOW(), INTERVAL 2 DAY), NULL),
(2, 5, 0, 72, DATE_SUB(NOW(), INTERVAL 3 DAY), NULL),
(5, 4, 1, 84, DATE_SUB(NOW(), INTERVAL 1 DAY), 'Ожидания по зарплате совпадают.'),
(4, 3, 1, 88, DATE_SUB(NOW(), INTERVAL 2 DAY), 'Сильное портфолио.'),
(3, 2, 1, 90, DATE_SUB(NOW(), INTERVAL 4 DAY), NULL);

INSERT INTO `Interviewers` (`Id`, `FullName`, `Email`, `Position`) VALUES
(1, 'Екатерина Орлова', 'ekaterina.orlova@hrms.local', 'HR Business Partner'),
(2, 'Павел Кузнецов', 'pavel.kuznetsov@hrms.local', 'Tech Lead'),
(3, 'Марина Белова', 'marina.belova@hrms.local', 'Product Lead'),
(4, 'Сергей Волков', 'sergey.volkov@hrms.local', 'QA Lead');

INSERT INTO `Interviews` (`Id`, `ApplicationId`, `ScheduledAt`, `Mode`, `Result`, `Notes`) VALUES
(1, 1, DATE_ADD(NOW(), INTERVAL 1 DAY), 2, 0, 'Техническое интервью: архитектура и EF Core.'),
(2, 3, DATE_ADD(NOW(), INTERVAL 2 DAY), 0, 0, 'Финальная встреча с командой HR.'),
(3, 4, DATE_SUB(NOW(), INTERVAL 3 DAY), 2, 1, 'Портфолио и работа с дизайн-системой.'),
(4, 6, DATE_SUB(NOW(), INTERVAL 10 DAY), 1, 2, 'Обсудили ожидания по формату работы.'),
(5, 2, DATE_ADD(NOW(), INTERVAL 3 DAY), 0, 0, NULL);

INSERT INTO `InterviewInterviewers` (`InterviewId`, `InterviewerId`, `Role`) VALUES
(1, 2, 'Technical'),
(1, 1, 'HR'),
(2, 1, 'Host'),
(3, 3, 'Host'),
(4, 4, 'Technical'),
(5, 2, 'Technical'),
(5, 1, 'HR');

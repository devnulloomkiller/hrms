-- HRMS: заполнение таблиц демонстрационными данными (MySQL 8.4).
-- Выполнять после 01_create_tables.sql.

INSERT INTO Candidates (LastName, FirstName, Email, Phone) VALUES
    ('Ivanov',   'Ivan',    'ivan.ivanov@example.com',    '+375 29 111-22-33'),
    ('Petrov',   'Petr',    'petr.petrov@example.com',    '+375 29 222-33-44'),
    ('Sidorova', 'Anna',    'anna.sidorova@example.com',  '+375 33 333-44-55'),
    ('Kozlov',   'Dmitry',  'dmitry.kozlov@example.com',  '+375 29 444-55-66'),
    ('Smirnova', 'Elena',   'elena.smirnova@example.com', '+375 25 555-66-77');

INSERT INTO Vacancies (Title, Department, Description, SalaryMin, SalaryMax, Status, PublishedAt, AcceptingResumesUntil) VALUES
    ('Senior .NET Developer',   'IT',              'Backend development of internal systems.',  5000, 8000, 1, '2026-10-01', '2026-11-20'),
    ('HR Manager',              'Human Resources', 'Full-cycle recruitment.',                   2500, 3500, 1, '2026-10-05', '2026-11-18'),
    ('QA Engineer',             'IT',              'Manual and automated testing.',             3000, 4500, 1, '2026-10-10', '2026-11-30'),
    ('Data Analyst',            'Analytics',       'BI reports and dashboards.',                3500, 5000, 2, '2026-09-15', '2026-10-31'),
    ('DevOps Engineer',         'IT',              'CI/CD, Kubernetes.',                        6000, NULL, 1, '2026-11-01', '2026-12-15');

INSERT INTO Resumes (CandidateId, VacancyId, Summary, ExperienceYears, SubmittedAt, Status) VALUES
    (1, 1, '.NET developer, 7 years, ASP.NET Core, SQL.',     7, '2026-10-03 10:00:00', 1),
    (1, 3, 'Some QA automation experience, Selenium.',        2, '2026-10-12 12:30:00', 2),
    (2, 1, '.NET developer, 4 years, web services.',          4, '2026-10-07 09:15:00', 1),
    (2, 5, 'Linux administration, Docker, K8s basics.',       5, '2026-11-02 14:45:00', 0),
    (3, 2, 'HR generalist, 6 years, IT recruitment.',         6, '2026-10-08 11:20:00', 3),
    (4, 3, 'QA engineer, 3 years, automated testing.',        3, '2026-10-15 16:00:00', 1),
    (4, 4, 'SQL, PowerBI, basic statistics.',                 4, '2026-09-28 13:10:00', 2),
    (5, 2, 'HR specialist, 3 years, sourcing and screening.', 3, '2026-10-20 10:50:00', 0),
    (5, 4, 'Analyst: Excel, SQL, Tableau.',                   5, '2026-09-25 15:35:00', 1);

INSERT INTO ResumeDetails (ResumeId, Education, Skills, FilePath) VALUES
    (1, 'BSU, Computer Science (2019)',    'C#, ASP.NET Core, EF Core, PostgreSQL, Docker',  '/files/resume_1.pdf'),
    (3, 'BSUIR, Informatics (2022)',       'C#, .NET, REST, MS SQL',                         '/files/resume_3.pdf'),
    (5, 'BSEU, HR Management (2018)',      'recruitment, onboarding, HRIS, interviewing',    '/files/resume_5.pdf'),
    (6, 'BSUIR, Software Testing (2021)',  'Selenium, Playwright, API testing, SQL',         '/files/resume_6.pdf'),
    (9, 'BSU, Applied Mathematics (2020)', 'SQL, Tableau, Excel, Python',                    '/files/resume_9.pdf');

INSERT INTO Interviews (ResumeId, VacancyId, ScheduledAt, Mode, Result, Notes) VALUES
    (1, 1, '2026-10-10 12:00:00', 2, 1, 'Strong backend fundamentals.'),
    (1, 1, '2026-10-17 15:00:00', 0, 1, 'Onsite final: system design.'),
    (2, 3, '2026-10-20 11:00:00', 1, 2, 'Not enough automation depth.'),
    (3, 1, '2026-10-12 10:30:00', 2, 1, 'Good mid-level profile.'),
    (3, 1, '2026-10-19 10:30:00', 0, 0, 'Final scheduled: team fit.'),
    (5, 2, '2026-10-15 09:00:00', 0, 1, 'Relevant IT recruitment experience.'),
    (6, 3, '2026-10-25 13:00:00', 2, 0, 'Tech screening.'),
    (9, 4, '2026-10-05 14:00:00', 2, 1, 'Good SQL and dashboards.');

INSERT INTO Interviewers (FullName, Email, Position) VALUES
    ('Aliaksei Kavalchuk', 'a.kavalchuk@hrms.local',  'Team Lead, Backend'),
    ('Maryna Savitskaya',  'm.savitskaya@hrms.local', 'HR Director'),
    ('Vadzim Lisitsa',     'v.lisitsa@hrms.local',    'Senior .NET Developer'),
    ('Olga Novik',         'o.novik@hrms.local',      'QA Lead'),
    ('Siarhei Baranau',    's.baranau@hrms.local',    'Data Engineering Lead');

INSERT INTO InterviewInterviewers (InterviewId, InterviewerId, Role) VALUES
    (1, 1, 'Host'),      (1, 3, 'Technical'),
    (2, 1, 'Host'),
    (3, 4, 'Technical'),
    (4, 3, 'Technical'), (4, 2, 'HR'),
    (5, 1, 'Host'),      (5, 2, 'HR'),
    (6, 2, 'Host'),
    (7, 4, 'Technical'), (7, 1, 'Observer'),
    (8, 5, 'Technical');

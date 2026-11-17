using Hrms.Domain.Entities;
using Hrms.Domain.Enums;
using Hrms.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Hrms.Infrastructure;

public static class DbInitializer
{
    public const string AdminRole = "Admin";
    public const string HrRole = "HR";

    public static async Task SeedRolesAndAdminAsync(
        RoleManager<IdentityRole> roles, UserManager<ApplicationUser> users)
    {
        foreach (var role in new[] { AdminRole, HrRole })
            if (!await roles.RoleExistsAsync(role))
                await roles.CreateAsync(new IdentityRole(role));

        const string adminEmail = "admin@hrms.local";
        const string adminPassword = "Admin123";
        var admin = await users.FindByEmailAsync(adminEmail);
        if (admin is null)
        {
            admin = new ApplicationUser { UserName = adminEmail, Email = adminEmail, EmailConfirmed = true, DisplayName = "Administrator" };
            var result = await users.CreateAsync(admin, adminPassword);
            if (result.Succeeded)
                await users.AddToRoleAsync(admin, AdminRole);
        }
    }

    /// <summary>
    /// Демо-набор для пустой базы: вакансии, кандидаты с резюме, отклики, подборы и собеседования.
    /// Повторный запуск ничего не меняет — данные добавляются только когда база пуста.
    /// </summary>
    public static async Task SeedDemoDataAsync(ApplicationDbContext db)
    {
        if (await db.Candidates.AnyAsync() || await db.Vacancies.AnyAsync()) return;

        var today = DateOnly.FromDateTime(DateTime.Today);
        var now = DateTime.Now;

        Candidate Person(string lastName, string firstName, string email, string phone,
            string headline, int experienceYears, decimal desiredSalary,
            string skills, string education, string summary) =>
            new()
            {
                LastName = lastName,
                FirstName = firstName,
                Email = email,
                Phone = phone,
                Resume = new Resume
                {
                    Headline = headline,
                    Summary = summary,
                    ExperienceYears = experienceYears,
                    DesiredSalary = desiredSalary,
                    CreatedAt = now,
                    UpdatedAt = now,
                    Details = new ResumeDetails
                    {
                        Skills = skills,
                        Education = education,
                    },
                },
            };

        var candidates = new List<Candidate>
        {
            Person("Иванова", "Анна", "anna.ivanova@example.com", "+375 29 111-22-33",
                "Senior .NET Developer", 7, 380000,
                "C#, ASP.NET Core, EF Core, SQL, Docker",
                "БГУ, прикладная математика, 2017",
                "Веду разработку внутренних сервисов на .NET, отвечаю за архитектуру и код-ревью."),
            Person("Петров", "Максим", "maksim.petrov@example.com", "+375 29 222-33-44",
                ".NET Developer", 4, 280000,
                "C#, ASP.NET Core, React, SQL",
                "БГУИР, программная инженерия, 2020",
                "Разрабатываю бизнес-приложения на ASP.NET Core, есть опыт с React и CI."),
            Person("Смирнова", "Ольга", "olga.smirnova@example.com", "+375 29 333-44-55",
                "HR Generalist", 5, 150000,
                "Подбор, HR-аналитика, интервью, адаптация",
                "БГЭУ, управление персоналом, 2019",
                "Веду полный цикл подбора: от брифа до оффера, выстраиваю процессы адаптации."),
            Person("Козлов", "Дмитрий", "dmitry.kozlov@example.com", "+375 29 444-55-66",
                "Product Designer", 6, 230000,
                "Figma, UX research, дизайн-системы, прототипирование",
                "БГАИ, дизайн, 2018",
                "Проектирую интерфейсы B2B-продуктов, развиваю дизайн-систему и исследую пользователей."),
            Person("Соколова", "Елена", "elena.sokolova@example.com", "+375 29 555-66-77",
                "Data Analyst", 3, 190000,
                "SQL, Python, Power BI, A/B-тесты",
                "БГУ, экономика, 2021",
                "Собираю продуктовые метрики, строю отчёты и проверяю гипотезы на данных."),
            Person("Лебедев", "Игорь", "igor.lebedev@example.com", "+375 29 666-77-88",
                "QA Engineer", 8, 170000,
                "Playwright, Selenium, API testing, Postman",
                "БНТУ, информационные системы, 2016",
                "Отвечаю за тестовую стратегию и автотесты API и UI."),
        };
        db.Candidates.AddRange(candidates);
        await db.SaveChangesAsync();

        var vacancies = new List<Vacancy>
        {
            new()
            {
                Title = "Senior .NET Developer", Department = "Разработка",
                Description = "Разработка платформы внутренних сервисов, код-ревью и наставничество.",
                SalaryMin = 280000, SalaryMax = 420000, MinExperience = 5,
                RequiredSkills = "C#, ASP.NET Core, EF Core, SQL",
                Status = VacancyStatus.Open,
                PublishedAt = today.AddDays(-14), AcceptingResumesUntil = today.AddDays(12),
            },
            new()
            {
                Title = "HR Generalist", Department = "HR",
                Description = "Полный цикл подбора на технические и продуктовые роли.",
                SalaryMin = 120000, SalaryMax = 160000, MinExperience = 3,
                RequiredSkills = "Подбор, HR-аналитика, интервью",
                Status = VacancyStatus.Open,
                PublishedAt = today.AddDays(-10), AcceptingResumesUntil = today.AddDays(5),
            },
            new()
            {
                Title = "Product Designer", Department = "Продукт",
                Description = "Дизайн B2B-интерфейсов и развитие дизайн-системы.",
                SalaryMin = 180000, SalaryMax = 250000, MinExperience = 4,
                RequiredSkills = "Figma, UX research, дизайн-системы",
                Status = VacancyStatus.Open,
                PublishedAt = today.AddDays(-7), AcceptingResumesUntil = today.AddDays(20),
            },
            new()
            {
                Title = "Data Analyst", Department = "Аналитика",
                Description = "Продуктовая аналитика и отчётность для команд.",
                SalaryMin = 160000, SalaryMax = 220000, MinExperience = 2,
                RequiredSkills = "SQL, Python, Power BI",
                Status = VacancyStatus.Paused,
                PublishedAt = today.AddDays(-20), AcceptingResumesUntil = today.AddDays(30),
            },
            new()
            {
                Title = "QA Engineer", Department = "Разработка",
                Description = "Тестовая стратегия, API- и UI-автотесты.",
                SalaryMin = 130000, SalaryMax = 180000, MinExperience = 3,
                RequiredSkills = "Playwright, Selenium, API testing",
                Status = VacancyStatus.Closed,
                PublishedAt = today.AddDays(-40), AcceptingResumesUntil = today.AddDays(-5),
            },
        };
        db.Vacancies.AddRange(vacancies);
        await db.SaveChangesAsync();

        var interviewers = new List<Interviewer>
        {
            new() { FullName = "Екатерина Орлова", Email = "ekaterina.orlova@hrms.local", Position = "HR Business Partner" },
            new() { FullName = "Павел Кузнецов", Email = "pavel.kuznetsov@hrms.local", Position = "Tech Lead" },
            new() { FullName = "Марина Белова", Email = "marina.belova@hrms.local", Position = "Product Lead" },
            new() { FullName = "Сергей Волков", Email = "sergey.volkov@hrms.local", Position = "QA Lead" },
        };
        db.Interviewers.AddRange(interviewers);
        await db.SaveChangesAsync();

        var anna = candidates[0];
        var maxim = candidates[1];
        var olga = candidates[2];
        var dmitry = candidates[3];
        var elena = candidates[4];
        var igor = candidates[5];
        var dotnet = vacancies[0];
        var hr = vacancies[1];
        var designer = vacancies[2];
        var analyst = vacancies[3];
        var qa = vacancies[4];

        var applications = new List<JobApplication>
        {
            new() { Candidate = anna, Vacancy = dotnet, Stage = ApplicationStage.Interview, AppliedAt = now.AddDays(-6), Source = "Карьерный сайт", Notes = "Сильное портфолио, назначено техническое интервью." },
            new() { Candidate = maxim, Vacancy = dotnet, Stage = ApplicationStage.Screening, AppliedAt = now.AddDays(-4), Source = "LinkedIn" },
            new() { Candidate = olga, Vacancy = hr, Stage = ApplicationStage.Offer, AppliedAt = now.AddDays(-8), Source = "Рекомендация" },
            new() { Candidate = dmitry, Vacancy = designer, Stage = ApplicationStage.Interview, AppliedAt = now.AddDays(-3), Source = "Карьерный сайт" },
            new() { Candidate = elena, Vacancy = analyst, Stage = ApplicationStage.Applied, AppliedAt = now.AddDays(-2), Source = "hh.ru" },
            new() { Candidate = igor, Vacancy = qa, Stage = ApplicationStage.Rejected, AppliedAt = now.AddDays(-20), Source = "hh.ru", Notes = "Не совпал формат работы." },
            new() { Candidate = maxim, Vacancy = qa, Stage = ApplicationStage.Withdrawn, AppliedAt = now.AddDays(-18), Source = "LinkedIn" },
        };
        db.JobApplications.AddRange(applications);
        await db.SaveChangesAsync();

        db.ResumeVacancyMatches.AddRange(
            new ResumeVacancyMatch { Resume = anna.Resume!, Vacancy = designer, Status = MatchStatus.Suggested, MatchScore = 60, MatchedAt = now.AddDays(-2) },
            new ResumeVacancyMatch { Resume = maxim.Resume!, Vacancy = qa, Status = MatchStatus.Suggested, MatchScore = 72, MatchedAt = now.AddDays(-3) },
            new ResumeVacancyMatch { Resume = elena.Resume!, Vacancy = analyst, Status = MatchStatus.Shortlisted, MatchScore = 84, MatchedAt = now.AddDays(-1) },
            new ResumeVacancyMatch { Resume = dmitry.Resume!, Vacancy = designer, Status = MatchStatus.Shortlisted, MatchScore = 88, MatchedAt = now.AddDays(-2) },
            new ResumeVacancyMatch { Resume = olga.Resume!, Vacancy = hr, Status = MatchStatus.Shortlisted, MatchScore = 90, MatchedAt = now.AddDays(-4) });
        await db.SaveChangesAsync();

        var annaApplication = applications[0];
        var olgaApplication = applications[2];
        var dmitryApplication = applications[3];
        var igorApplication = applications[5];
        var maximApplication = applications[1];

        var interviews = new List<Interview>
        {
            new()
            {
                Application = annaApplication,
                ScheduledAt = DateTime.Today.AddDays(1).AddHours(10),
                Mode = InterviewMode.Video,
                Result = InterviewResult.Scheduled,
                Notes = "Техническое интервью: архитектура и EF Core.",
                Participants =
                {
                    new InterviewInterviewer { Interviewer = interviewers[1], Role = "Technical" },
                    new InterviewInterviewer { Interviewer = interviewers[0], Role = "HR" },
                },
            },
            new()
            {
                Application = olgaApplication,
                ScheduledAt = DateTime.Today.AddDays(2).AddHours(14),
                Mode = InterviewMode.Onsite,
                Result = InterviewResult.Scheduled,
                Notes = "Финальная встреча с командой HR.",
                Participants =
                {
                    new InterviewInterviewer { Interviewer = interviewers[0], Role = "Host" },
                },
            },
            new()
            {
                Application = dmitryApplication,
                ScheduledAt = DateTime.Today.AddDays(-3).AddHours(11),
                Mode = InterviewMode.Video,
                Result = InterviewResult.Passed,
                Notes = "Портфолио и работа с дизайн-системой.",
                Participants =
                {
                    new InterviewInterviewer { Interviewer = interviewers[2], Role = "Host" },
                },
            },
            new()
            {
                Application = igorApplication,
                ScheduledAt = DateTime.Today.AddDays(-10).AddHours(15),
                Mode = InterviewMode.Phone,
                Result = InterviewResult.Failed,
                Notes = "Обсудили ожидания по формату работы.",
                Participants =
                {
                    new InterviewInterviewer { Interviewer = interviewers[3], Role = "Technical" },
                },
            },
            new()
            {
                Application = maximApplication,
                ScheduledAt = DateTime.Today.AddDays(3).AddHours(12),
                Mode = InterviewMode.Onsite,
                Result = InterviewResult.Scheduled,
                Participants =
                {
                    new InterviewInterviewer { Interviewer = interviewers[1], Role = "Technical" },
                    new InterviewInterviewer { Interviewer = interviewers[0], Role = "HR" },
                },
            },
        };
        db.Interviews.AddRange(interviews);
        await db.SaveChangesAsync();
    }
}

using System.Diagnostics;
using Hrms.Application.Interfaces;
using Hrms.Domain.Entities;
using Hrms.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace Hrms.Web.Controllers;

public class HomeController(
    IVacancyService vacancies,
    ICandidateService candidates,
    IResumeService resumes,
    IInterviewService interviews) : Controller
{
    public async Task<IActionResult> Index(CancellationToken ct)
    {
        if (User.Identity?.IsAuthenticated != true)
            return View(new DashboardViewModel());

        var allVacancies = await vacancies.GetAllAsync(ct);
        var allCandidates = await candidates.GetAllAsync(ct);
        var allResumes = await resumes.GetAllAsync(ct);
        var allInterviews = await interviews.GetAllAsync(ct);

        var now = DateTime.Now;
        var today = DateOnly.FromDateTime(now);
        var upcoming = allInterviews
            .Where(i => i.ScheduledAt >= now && i.Result == InterviewResult.Scheduled)
            .OrderBy(i => i.ScheduledAt)
            .ToList();

        return View(new DashboardViewModel
        {
            OpenVacancies = allVacancies.Count(v => v.Status == VacancyStatus.Open),
            TotalCandidates = allCandidates.Count,
            ActiveResumes = allResumes.Count(r => r.Status is ResumeStatus.New or ResumeStatus.InReview),
            UpcomingInterviewsCount = upcoming.Count,
            UpcomingInterviews = upcoming.Take(5).ToList(),
            ExpiringVacancies = allVacancies
                .Where(v => v.Status == VacancyStatus.Open
                    && v.AcceptingResumesUntil is { } until
                    && until >= today
                    && until.DayNumber <= today.AddDays(7).DayNumber)
                .OrderBy(v => v.AcceptingResumesUntil)
                .Take(5)
                .ToList(),
            RecentResumes = allResumes.Take(5).ToList(),
        });
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

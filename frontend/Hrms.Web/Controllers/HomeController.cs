using System.Diagnostics;
using Hrms.Application.Interfaces;
using Hrms.Domain.Enums;
using Hrms.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hrms.Web.Controllers;

[Authorize]
public class HomeController(
    IVacancyService vacancies,
    ICandidateService candidates,
    IJobApplicationService applications,
    IInterviewService interviews) : Controller
{
    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var allVacancies = await vacancies.GetAllAsync(null, ct);
        var allCandidates = await candidates.GetAllAsync(null, ct);
        var allApplications = await applications.GetAllAsync(null, ct);
        var allInterviews = await interviews.GetAllAsync(null, ct);

        var now = DateTime.Now;
        var today = DateOnly.FromDateTime(now);
        var upcoming = allInterviews
            .Where(i => i.ScheduledAt >= now && i.Result == InterviewResult.Scheduled)
            .OrderBy(i => i.ScheduledAt)
            .ToList();

        var pipeline = Enum.GetValues<ApplicationStage>()
            .ToDictionary(stage => stage, stage => allApplications.Count(a => a.Stage == stage));

        return View(new DashboardViewModel
        {
            OpenVacancies = allVacancies.Count(v => v.Status == VacancyStatus.Open),
            TotalCandidates = allCandidates.Count,
            ActiveApplications = allApplications.Count(a => a.Stage is
                ApplicationStage.Applied or ApplicationStage.Screening or ApplicationStage.Interview or ApplicationStage.Offer),
            UpcomingInterviewsCount = upcoming.Count,
            HiredCount = pipeline[ApplicationStage.Hired],
            UpcomingInterviews = upcoming.Take(5).ToList(),
            ExpiringVacancies = allVacancies
                .Where(v => v.Status == VacancyStatus.Open
                    && v.AcceptingResumesUntil is { } until
                    && until >= today
                    && until.DayNumber <= today.AddDays(7).DayNumber)
                .OrderBy(v => v.AcceptingResumesUntil)
                .Take(5)
                .ToList(),
            RecentApplications = allApplications.Take(5).ToList(),
            Pipeline = pipeline,
        });
    }

    [AllowAnonymous]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

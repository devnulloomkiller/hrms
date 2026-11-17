using Hrms.Application.DTOs;
using Hrms.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Hrms.Web.Controllers;

[Authorize]
public class ReportsController(IReportService reports, ICandidateService candidates) : Controller
{
    public async Task<IActionResult> ExpiringVacancies(int? days, CancellationToken ct)
    {
        ViewBag.Days = days ?? 14;
        var list = await reports.GetVacanciesWithExpiringDeadlineAsync(ViewBag.Days, ct);
        return View(list);
    }

    public async Task<IActionResult> CandidateProfile(int? id, CancellationToken ct)
    {
        ViewBag.Candidates = (await candidates.GetOptionsAsync(false, ct))
            .Select(c => new SelectListItem($"{c.FullName} · {c.Headline}", c.Id.ToString()))
            .ToList();

        if (id is null) return View((CandidateProfileDto?)null);
        var profile = await reports.GetCandidateProfileAsync(id.Value, ct);
        if (profile is null) return NotFound();
        return View(profile);
    }

    public async Task<IActionResult> InterviewSummary(DateTime? from, DateTime? to, CancellationToken ct)
    {
        ViewBag.From = (from ?? DateTime.Today.AddMonths(-2)).ToString("yyyy-MM-dd");
        ViewBag.To = (to ?? DateTime.Today.AddDays(1)).ToString("yyyy-MM-dd");
        var list = await reports.GetInterviewSummaryByVacancyAsync(
            DateTime.Parse(ViewBag.From), DateTime.Parse(ViewBag.To), ct);
        return View(list);
    }

    public async Task<IActionResult> Pipeline(CancellationToken ct) =>
        View(await reports.GetPipelineByVacancyAsync(ct));
}

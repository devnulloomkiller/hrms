using Microsoft.AspNetCore.Authorization;
using Hrms.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

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
        ViewBag.Candidates = (await candidates.GetAllAsync(ct))
            .Select(c => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem(
                $"{c.LastName} {c.FirstName}", c.Id.ToString()))
            .ToList();
        if (id is null) return View((Hrms.Application.DTOs.CandidateProfileDto?)null);
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
}

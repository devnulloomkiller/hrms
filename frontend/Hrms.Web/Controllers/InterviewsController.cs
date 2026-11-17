using Hrms.Application.Filters;
using Hrms.Application.Interfaces;
using Hrms.Domain.Entities;
using Hrms.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Hrms.Web.Controllers;

[Authorize(Roles = "Admin,HR")]
public class InterviewsController(
    IInterviewService interviews,
    IVacancyService vacancies) : Controller
{
    public async Task<IActionResult> Index(bool? upcoming, InterviewResult? result, int? vacancyId, CancellationToken ct)
    {
        ViewBag.Upcoming = upcoming;
        ViewBag.Result = result;
        ViewBag.VacancyId = vacancyId;
        ViewBag.Vacancies = await vacancies.GetOptionsAsync(ct);
        return View(await interviews.GetAllAsync(new InterviewFilter(upcoming, result, vacancyId), ct));
    }

    public async Task<IActionResult> Details(int? id, CancellationToken ct)
    {
        if (id is null) return NotFound();
        var interview = await interviews.GetByIdAsync(id.Value, ct);
        return interview is null ? NotFound() : View(interview);
    }

    public async Task<IActionResult> Create(int? applicationId, CancellationToken ct)
    {
        await FillSelectorsAsync(ct);
        return View("Form", new Interview
        {
            ApplicationId = applicationId ?? 0,
            ScheduledAt = DateTime.Now.AddDays(1),
            Mode = InterviewMode.Video,
            Result = InterviewResult.Scheduled,
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(int applicationId, DateTime scheduledAt,
        InterviewMode mode, InterviewResult result, string? notes, int[]? interviewerIds, string[]? roles, CancellationToken ct)
    {
        if (applicationId == 0 || interviewerIds is null || interviewerIds.Length == 0)
        {
            ModelState.AddModelError("", "Выберите отклик и хотя бы одного участника.");
            await FillSelectorsAsync(ct);
            return View("Form", BuildInterview(0, applicationId, scheduledAt, mode, result, notes, interviewerIds, roles));
        }

        var interview = BuildInterview(0, applicationId, scheduledAt, mode, result, notes, interviewerIds, roles);
        await interviews.AddAsync(interview, ct);
        TempData["StatusMessage"] = "Собеседование назначено. Этап отклика переведён в «Собеседования».";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int? id, CancellationToken ct)
    {
        if (id is null) return NotFound();
        var interview = await interviews.GetByIdAsync(id.Value, ct);
        if (interview is null) return NotFound();
        await FillSelectorsAsync(ct);
        return View("Form", interview);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, int applicationId, DateTime scheduledAt,
        InterviewMode mode, InterviewResult result, string? notes, int[]? interviewerIds, string[]? roles, CancellationToken ct)
    {
        var interview = BuildInterview(id, applicationId, scheduledAt, mode, result, notes, interviewerIds, roles);
        if (applicationId == 0 || interviewerIds is null || interviewerIds.Length == 0)
        {
            ModelState.AddModelError("", "Выберите отклик и хотя бы одного участника.");
            await FillSelectorsAsync(ct);
            return View("Form", interview);
        }

        try
        {
            await interviews.UpdateAsync(interview, ct);
        }
        catch (InvalidOperationException)
        {
            return NotFound();
        }

        TempData["StatusMessage"] = "Собеседование обновлено.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int? id, CancellationToken ct)
    {
        if (id is null) return NotFound();
        var interview = await interviews.GetByIdAsync(id.Value, ct);
        return interview is null ? NotFound() : View(interview);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken ct)
    {
        try
        {
            await interviews.DeleteAsync(id, ct);
            TempData["StatusMessage"] = "Собеседование удалено.";
        }
        catch (InvalidOperationException)
        {
            return NotFound();
        }

        return RedirectToAction(nameof(Index));
    }

    private static Interview BuildInterview(int id, int applicationId, DateTime scheduledAt,
        InterviewMode mode, InterviewResult result, string? notes, int[]? interviewerIds, string[]? roles)
    {
        var interview = new Interview
        {
            Id = id,
            ApplicationId = applicationId,
            ScheduledAt = scheduledAt,
            Mode = mode,
            Result = result,
            Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim(),
        };

        if (interviewerIds is not null)
            for (var i = 0; i < interviewerIds.Length; i++)
            {
                var role = roles is not null && i < roles.Length && !string.IsNullOrWhiteSpace(roles[i])
                    ? roles[i].Trim()
                    : "Interviewer";
                if (role.Length > 50) role = role[..50];

                interview.Participants.Add(new InterviewInterviewer
                {
                    InterviewerId = interviewerIds[i],
                    Role = role,
                });
            }

        return interview;
    }

    private async Task FillSelectorsAsync(CancellationToken ct)
    {
        ViewBag.Applications = await interviews.GetApplicationOptionsAsync(ct);
        ViewBag.Interviewers = (await interviews.GetInterviewersAsync(ct))
            .Select(i => new SelectListItem(i.FullName, i.Id.ToString()))
            .ToList();
    }
}

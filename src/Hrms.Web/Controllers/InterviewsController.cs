using Microsoft.AspNetCore.Authorization;
using Hrms.Application.Interfaces;
using Hrms.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Hrms.Web.Controllers;

[Authorize(Roles = "Admin,HR")]
public class InterviewsController(
    IInterviewService interviews,
    IResumeService resumes,
    IVacancyService vacancies) : Controller
{
    public async Task<IActionResult> Index(CancellationToken ct) =>
        View(await interviews.GetAllAsync(ct));

    public async Task<IActionResult> Details(int? id, CancellationToken ct)
    {
        if (id is null) return NotFound();
        var interview = await interviews.GetByIdAsync(id.Value, ct);
        return interview is null ? NotFound() : View(interview);
    }

    public async Task<IActionResult> Create(CancellationToken ct)
    {
        await FillSelectorsAsync(ct);
        return View("Form", new Interview { ScheduledAt = DateTime.Now.AddDays(1), Mode = InterviewMode.Onsite, Result = InterviewResult.Scheduled });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(int resumeId, int vacancyId, DateTime scheduledAt,
        InterviewMode mode, InterviewResult result, string? notes, int[] interviewerIds, string[] roles, CancellationToken ct)
    {
        if (resumeId == 0 || vacancyId == 0 || interviewerIds.Length == 0)
        {
            ModelState.AddModelError("", "Укажите резюме, вакансию и хотя бы одного участника.");
            await FillSelectorsAsync(ct);
            return View("Form", BuildInterview(0, resumeId, vacancyId, scheduledAt, mode, result, notes, interviewerIds, roles));
        }
        var interview = BuildInterview(0, resumeId, vacancyId, scheduledAt, mode, result, notes, interviewerIds, roles);
        await interviews.AddAsync(interview, ct);
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
    public async Task<IActionResult> Edit(int id, int resumeId, int vacancyId, DateTime scheduledAt,
        InterviewMode mode, InterviewResult result, string? notes, int[] interviewerIds, string[] roles, CancellationToken ct)
    {
        var interview = BuildInterview(id, resumeId, vacancyId, scheduledAt, mode, result, notes, interviewerIds, roles);
        if (resumeId == 0 || vacancyId == 0 || interviewerIds.Length == 0)
        {
            ModelState.AddModelError("", "Укажите резюме, вакансию и хотя бы одного участника.");
            await FillSelectorsAsync(ct);
            return View("Form", interview);
        }
        try { await interviews.UpdateAsync(interview, ct); }
        catch (InvalidOperationException) { return NotFound(); }
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
        try { await interviews.DeleteAsync(id, ct); }
        catch (InvalidOperationException) { return NotFound(); }
        return RedirectToAction(nameof(Index));
    }

    private static Interview BuildInterview(int id, int resumeId, int vacancyId, DateTime scheduledAt,
        InterviewMode mode, InterviewResult result, string? notes, int[] interviewerIds, string[] roles)
    {
        var interview = new Interview
        {
            Id = id,
            ResumeId = resumeId,
            VacancyId = vacancyId,
            ScheduledAt = scheduledAt,
            Mode = mode,
            Result = result,
            Notes = notes,
        };
        for (var i = 0; i < interviewerIds.Length; i++)
            interview.Participants.Add(new InterviewInterviewer
            {
                InterviewerId = interviewerIds[i],
                Role = i < roles.Length && !string.IsNullOrWhiteSpace(roles[i]) ? roles[i] : "Interviewer",
            });
        return interview;
    }

    private async Task FillSelectorsAsync(CancellationToken ct)
    {
        var resumeList = await resumes.GetAllAsync(ct);
        ViewBag.Resumes = resumeList.Select(r => new SelectListItem(
            $"#{r.Id} {r.Candidate.LastName} {r.Candidate.FirstName} → {r.Vacancy?.Title ?? "—"}", r.Id.ToString())).ToList();
        ViewBag.Vacancies = (await vacancies.GetAllAsync(ct))
            .Select(v => new SelectListItem(v.Title, v.Id.ToString())).ToList();
        ViewBag.Interviewers = (await interviews.GetInterviewersAsync(ct))
            .Select(i => new SelectListItem(i.FullName, i.Id.ToString())).ToList();
    }
}

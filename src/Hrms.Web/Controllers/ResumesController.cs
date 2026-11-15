using Microsoft.AspNetCore.Authorization;
using Hrms.Application.Interfaces;
using Hrms.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Hrms.Web.Controllers;

[Authorize(Roles = "Admin,HR")]
public class ResumesController(
    IResumeService resumes,
    ICandidateService candidates,
    IVacancyService vacancies) : Controller
{
    public async Task<IActionResult> Index(CancellationToken ct) =>
        View(await resumes.GetAllAsync(ct));

    public async Task<IActionResult> Details(int? id, CancellationToken ct)
    {
        if (id is null) return NotFound();
        var resume = await resumes.GetByIdAsync(id.Value, ct);
        return resume is null ? NotFound() : View(resume);
    }

    public async Task<IActionResult> Create(CancellationToken ct)
    {
        await FillSelectorsAsync(ct);
        return View("Form", new Resume { Summary = "", SubmittedAt = DateTime.Now, Status = ResumeStatus.New });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("CandidateId,VacancyId,Summary,ExperienceYears,SubmittedAt,Status")] Resume resume, CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            await FillSelectorsAsync(ct);
            return View("Form", resume);
        }
        await resumes.AddAsync(resume, ct);
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int? id, CancellationToken ct)
    {
        if (id is null) return NotFound();
        var resume = await resumes.GetByIdAsync(id.Value, ct);
        if (resume is null) return NotFound();
        await FillSelectorsAsync(ct);
        return View("Form", resume);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,CandidateId,VacancyId,Summary,ExperienceYears,SubmittedAt,Status")] Resume resume, CancellationToken ct)
    {
        if (id != resume.Id) return NotFound();
        if (!ModelState.IsValid)
        {
            await FillSelectorsAsync(ct);
            return View("Form", resume);
        }
        try { await resumes.UpdateAsync(resume, ct); }
        catch (InvalidOperationException) { return NotFound(); }
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int? id, CancellationToken ct)
    {
        if (id is null) return NotFound();
        var resume = await resumes.GetByIdAsync(id.Value, ct);
        return resume is null ? NotFound() : View(resume);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken ct)
    {
        try { await resumes.DeleteAsync(id, ct); }
        catch (InvalidOperationException) { return NotFound(); }
        return RedirectToAction(nameof(Index));
    }

    private async Task FillSelectorsAsync(CancellationToken ct)
    {
        ViewBag.Candidates = (await candidates.GetAllAsync(ct))
            .Select(c => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem(
                $"{c.LastName} {c.FirstName} ({c.Email})", c.Id.ToString()))
            .ToList();
        ViewBag.Vacancies = (await vacancies.GetAllAsync(ct))
            .Select(v => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem(v.Title, v.Id.ToString()))
            .ToList();
    }
}

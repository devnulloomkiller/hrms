using Hrms.Application.Filters;
using Hrms.Application.Interfaces;
using Hrms.Domain.Entities;
using Hrms.Domain.Enums;
using Hrms.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Hrms.Web.Controllers;

[Authorize(Roles = "Admin,HR")]
public class CandidatesController(
    ICandidateService candidates,
    IResumeService resumes) : Controller
{
    public async Task<IActionResult> Index(string? search, int? minExperience, string? skill, bool onlyWithResume, CancellationToken ct)
    {
        var filter = new CandidateFilter(search, minExperience, skill, onlyWithResume);
        ViewBag.Search = search;
        ViewBag.MinExperience = minExperience;
        ViewBag.Skill = skill;
        ViewBag.OnlyWithResume = onlyWithResume;
        return View(await candidates.GetAllAsync(filter, ct));
    }

    public async Task<IActionResult> Details(int? id, CancellationToken ct)
    {
        if (id is null) return NotFound();
        var candidate = await candidates.GetByIdAsync(id.Value, ct);
        return candidate is null ? NotFound() : View(candidate);
    }

    public IActionResult Create() => View("Form", new CandidateFormViewModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CandidateFormViewModel form, CancellationToken ct)
    {
        if (!ModelState.IsValid) return View("Form", form);

        try
        {
            await candidates.AddAsync(ToEntity(form), ct);
        }
        catch (DbUpdateException)
        {
            ModelState.AddModelError(nameof(form.Email), "Кандидат с таким email уже есть в картотеке.");
            return View("Form", form);
        }

        TempData["StatusMessage"] = $"Кандидат {form.LastName} {form.FirstName} добавлен.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int? id, CancellationToken ct)
    {
        if (id is null) return NotFound();
        var candidate = await candidates.GetByIdAsync(id.Value, ct);
        return candidate is null ? NotFound() : View("Form", ToForm(candidate));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, CandidateFormViewModel form, CancellationToken ct)
    {
        if (id != form.Id) return NotFound();
        if (!ModelState.IsValid) return View("Form", form);

        try
        {
            await candidates.UpdateAsync(ToEntity(form), ct);
        }
        catch (InvalidOperationException)
        {
            return NotFound();
        }
        catch (DbUpdateException)
        {
            ModelState.AddModelError(nameof(form.Email), "Кандидат с таким email уже есть в картотеке.");
            return View("Form", form);
        }

        TempData["StatusMessage"] = "Карточка кандидата обновлена.";
        return RedirectToAction(nameof(Details), new { id });
    }

    public async Task<IActionResult> Delete(int? id, CancellationToken ct)
    {
        if (id is null) return NotFound();
        var candidate = await candidates.GetByIdAsync(id.Value, ct);
        return candidate is null ? NotFound() : View(candidate);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken ct)
    {
        try
        {
            await candidates.DeleteAsync(id, ct);
        }
        catch (InvalidOperationException)
        {
            return NotFound();
        }

        TempData["StatusMessage"] = "Кандидат удалён вместе с резюме, откликами и собеседованиями.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Match(int? id, string? search, bool? onlyOpen, bool? onlyNotLinked, CancellationToken ct)
    {
        if (id is null) return NotFound();
        var candidate = await candidates.GetByIdAsync(id.Value, ct);
        if (candidate is null) return NotFound();
        if (candidate.Resume is null)
        {
            TempData["ErrorMessage"] = "Сначала заполните резюме кандидата.";
            return RedirectToAction(nameof(Edit), new { id });
        }

        var open = onlyOpen ?? true;
        var notLinked = onlyNotLinked ?? true;
        var filter = new VacancyMatchFilter(search, open, true, notLinked);
        ViewBag.Search = search;
        ViewBag.OnlyOpen = open;
        ViewBag.OnlyNotLinked = notLinked;
        ViewBag.Candidate = candidate;
        return View(await resumes.FindVacanciesForResumeAsync(candidate.Resume.Id, filter, ct));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Match(int id, int[]? vacancyIds, CancellationToken ct)
    {
        var candidate = await candidates.GetByIdAsync(id, ct);
        if (candidate?.Resume is null) return NotFound();

        if (vacancyIds is null || vacancyIds.Length == 0)
        {
            TempData["ErrorMessage"] = "Выберите хотя бы одну вакансию.";
            return RedirectToAction(nameof(Match), new { id });
        }

        try
        {
            var added = await resumes.LinkVacanciesToResumeAsync(candidate.Resume.Id, vacancyIds, MatchStatus.Suggested, ct);
            TempData["StatusMessage"] = added == 0
                ? "Выбранные вакансии уже есть в подборке."
                : $"В подборку добавлено вакансий: {added}.";
        }
        catch (InvalidOperationException ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }

        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RemoveMatch(int id, int vacancyId, CancellationToken ct)
    {
        var candidate = await candidates.GetByIdAsync(id, ct);
        if (candidate?.Resume is null) return NotFound();

        await resumes.UnlinkAsync(vacancyId, candidate.Resume.Id, ct);
        TempData["StatusMessage"] = "Вакансия убрана из подборки.";
        return RedirectToAction(nameof(Details), new { id });
    }

    private static Candidate ToEntity(CandidateFormViewModel form) => new()
    {
        Id = form.Id,
        LastName = form.LastName.Trim(),
        FirstName = form.FirstName.Trim(),
        Email = form.Email.Trim(),
        Phone = Normalize(form.Phone),
        Resume = new Resume
        {
            CandidateId = form.Id,
            Headline = form.Headline.Trim(),
            Summary = form.Summary.Trim(),
            ExperienceYears = form.ExperienceYears,
            DesiredSalary = form.DesiredSalary,
            Details = new ResumeDetails
            {
                Education = Normalize(form.Education),
                Skills = Normalize(form.Skills),
                FilePath = Normalize(form.FilePath),
            },
        },
    };

    private static CandidateFormViewModel ToForm(Candidate candidate) => new()
    {
        Id = candidate.Id,
        LastName = candidate.LastName,
        FirstName = candidate.FirstName,
        Email = candidate.Email,
        Phone = candidate.Phone,
        Headline = candidate.Resume?.Headline ?? "",
        Summary = candidate.Resume?.Summary ?? "",
        ExperienceYears = candidate.Resume?.ExperienceYears ?? 0,
        DesiredSalary = candidate.Resume?.DesiredSalary,
        Education = candidate.Resume?.Details?.Education,
        Skills = candidate.Resume?.Details?.Skills,
        FilePath = candidate.Resume?.Details?.FilePath,
    };

    private static string? Normalize(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}

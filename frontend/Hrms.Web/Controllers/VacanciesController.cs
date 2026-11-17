using Hrms.Application.Filters;
using Hrms.Application.Interfaces;
using Hrms.Domain.Entities;
using Hrms.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hrms.Web.Controllers;

[Authorize(Roles = "Admin,HR")]
public class VacanciesController(
    IVacancyService vacancies,
    IResumeService resumes) : Controller
{
    public async Task<IActionResult> Index(string? search, VacancyStatus? status, bool onlyAccepting, CancellationToken ct)
    {
        ViewBag.Search = search;
        ViewBag.Status = status;
        ViewBag.OnlyAccepting = onlyAccepting;
        return View(await vacancies.GetAllAsync(new VacancyFilter(search, status, onlyAccepting), ct));
    }

    public async Task<IActionResult> Details(int? id, CancellationToken ct)
    {
        if (id is null) return NotFound();
        var vacancy = await vacancies.GetByIdAsync(id.Value, ct);
        return vacancy is null ? NotFound() : View(vacancy);
    }

    public IActionResult Create() => View(new Vacancy
    {
        Title = "",
        Department = "",
        PublishedAt = DateOnly.FromDateTime(DateTime.Today),
        Status = VacancyStatus.Open,
    });

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Title,Department,Description,SalaryMin,SalaryMax,MinExperience,RequiredSkills,Status,PublishedAt,AcceptingResumesUntil")] Vacancy vacancy, CancellationToken ct)
    {
        ValidateVacancy(vacancy);
        if (!ModelState.IsValid) return View(vacancy);
        await vacancies.AddAsync(vacancy, ct);
        TempData["StatusMessage"] = $"Вакансия «{vacancy.Title}» создана.";
        return RedirectToAction(nameof(Details), new { id = vacancy.Id });
    }

    public async Task<IActionResult> Edit(int? id, CancellationToken ct)
    {
        if (id is null) return NotFound();
        var vacancy = await vacancies.GetByIdAsync(id.Value, ct);
        return vacancy is null ? NotFound() : View(vacancy);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Department,Description,SalaryMin,SalaryMax,MinExperience,RequiredSkills,Status,PublishedAt,AcceptingResumesUntil")] Vacancy vacancy, CancellationToken ct)
    {
        if (id != vacancy.Id) return NotFound();
        ValidateVacancy(vacancy);
        if (!ModelState.IsValid) return View(vacancy);

        try
        {
            await vacancies.UpdateAsync(vacancy, ct);
        }
        catch (InvalidOperationException)
        {
            return NotFound();
        }

        TempData["StatusMessage"] = "Вакансия обновлена.";
        return RedirectToAction(nameof(Details), new { id });
    }

    public async Task<IActionResult> Delete(int? id, CancellationToken ct)
    {
        if (id is null) return NotFound();
        var vacancy = await vacancies.GetByIdAsync(id.Value, ct);
        return vacancy is null ? NotFound() : View(vacancy);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken ct)
    {
        try
        {
            await vacancies.DeleteAsync(id, ct);
            TempData["StatusMessage"] = "Вакансия удалена.";
            return RedirectToAction(nameof(Index));
        }
        catch (InvalidOperationException ex)
        {
            TempData["ErrorMessage"] = ex.Message;
            return RedirectToAction(nameof(Details), new { id });
        }
    }

    public async Task<IActionResult> Match(int? id, int? minExperience, string? skill, decimal? maxDesiredSalary,
        bool? onlyNotLinked, bool? onlyOpenVacancy, CancellationToken ct)
    {
        if (id is null) return NotFound();
        var vacancy = await vacancies.GetByIdAsync(id.Value, ct);
        if (vacancy is null) return NotFound();

        var notLinked = onlyNotLinked ?? true;
        var openVacancy = onlyOpenVacancy ?? true;
        var filter = new ResumeMatchFilter(minExperience, skill, maxDesiredSalary, openVacancy, notLinked);
        ViewBag.MinExperience = minExperience;
        ViewBag.Skill = skill;
        ViewBag.MaxDesiredSalary = maxDesiredSalary;
        ViewBag.OnlyNotLinked = notLinked;
        ViewBag.OnlyOpenVacancy = openVacancy;
        ViewBag.Vacancy = vacancy;
        return View(await resumes.FindResumesForVacancyAsync(vacancy.Id, filter, ct));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Match(int id, int[]? resumeIds, MatchStatus status, CancellationToken ct)
    {
        if (resumeIds is null || resumeIds.Length == 0)
        {
            TempData["ErrorMessage"] = "Выберите хотя бы одно резюме.";
            return RedirectToAction(nameof(Match), new { id });
        }

        try
        {
            var added = await resumes.LinkResumesToVacancyAsync(id, resumeIds, status, ct);
            TempData["StatusMessage"] = added == 0
                ? "Выбранные резюме уже есть в подборке."
                : $"В подборку добавлено резюме: {added}.";
        }
        catch (InvalidOperationException ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }

        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateMatchStatus(int id, int resumeId, MatchStatus status, CancellationToken ct)
    {
        try
        {
            await resumes.UpdateMatchStatusAsync(id, resumeId, status, ct);
            TempData["StatusMessage"] = "Статус подбора обновлён.";
        }
        catch (InvalidOperationException)
        {
            TempData["ErrorMessage"] = "Подбор не найден.";
        }

        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Unlink(int id, int resumeId, CancellationToken ct)
    {
        await resumes.UnlinkAsync(id, resumeId, ct);
        TempData["StatusMessage"] = "Резюме убрано из подборки.";
        return RedirectToAction(nameof(Details), new { id });
    }

    private void ValidateVacancy(Vacancy vacancy)
    {
        if (vacancy.SalaryMax is { } max && max < vacancy.SalaryMin)
            ModelState.AddModelError(nameof(Vacancy.SalaryMax), "Зарплата «до» не может быть меньше «от».");

        if (vacancy.AcceptingResumesUntil is { } until && until < vacancy.PublishedAt)
            ModelState.AddModelError(nameof(Vacancy.AcceptingResumesUntil), "Срок приёма не может быть раньше даты публикации.");
    }
}

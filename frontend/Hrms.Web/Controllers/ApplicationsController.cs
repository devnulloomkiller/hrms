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
public class ApplicationsController(
    IJobApplicationService applications,
    ICandidateService candidates,
    IVacancyService vacancies) : Controller
{
    private static readonly ApplicationStage[] BoardStages =
    [
        ApplicationStage.Applied,
        ApplicationStage.Screening,
        ApplicationStage.Interview,
        ApplicationStage.Offer,
        ApplicationStage.Hired,
    ];

    public async Task<IActionResult> Index(string? search, int? vacancyId, ApplicationStage? stage, CancellationToken ct)
    {
        var list = await applications.GetAllAsync(new ApplicationFilter(search, vacancyId, stage), ct);
        var columns = BoardStages
            .Where(boardStage => stage is null || stage == boardStage)
            .Select(boardStage => new ApplicationColumn(
                boardStage,
                StageTitle(boardStage),
                list.Where(a => a.Stage == boardStage).ToList()))
            .ToList();

        var closed = list
            .Where(a => a.Stage is ApplicationStage.Rejected or ApplicationStage.Withdrawn)
            .ToList();

        var model = new ApplicationBoardViewModel
        {
            Columns = columns,
            ClosedApplications = closed,
            Vacancies = await vacancies.GetOptionsAsync(ct),
            Search = search,
            VacancyId = vacancyId,
            Stage = stage,
            TotalCount = list.Count,
        };
        return View(model);
    }

    public async Task<IActionResult> Create(int? candidateId, int? vacancyId, CancellationToken ct)
    {
        await FillSelectorsAsync(ct);
        return View("Form", new JobApplication
        {
            CandidateId = candidateId ?? 0,
            VacancyId = vacancyId ?? 0,
            Stage = ApplicationStage.Applied,
            AppliedAt = DateTime.Now,
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("CandidateId,VacancyId,Stage,AppliedAt,Source,Notes")] JobApplication application, CancellationToken ct)
    {
        if (application.CandidateId == 0 || application.VacancyId == 0)
            ModelState.AddModelError("", "Выберите кандидата и вакансию.");

        // Навигационные свойства не приходят из формы: без этого implicit required
        // для non-nullable Candidate/Vacancy блокирует любое сохранение.
        ModelState.Remove(nameof(JobApplication.Candidate));
        ModelState.Remove(nameof(JobApplication.Vacancy));

        if (!ModelState.IsValid)
        {
            await FillSelectorsAsync(ct);
            return View("Form", application);
        }

        try
        {
            await applications.AddAsync(application, ct);
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError("", ex.Message);
            await FillSelectorsAsync(ct);
            return View("Form", application);
        }
        catch (DbUpdateException)
        {
            ModelState.AddModelError("", "Не удалось сохранить отклик: возможно, он уже существует.");
            await FillSelectorsAsync(ct);
            return View("Form", application);
        }

        TempData["StatusMessage"] = "Отклик добавлен в воронку.";
        return RedirectToAction(nameof(Index), new { vacancyId = application.VacancyId });
    }

    public async Task<IActionResult> Edit(int? id, CancellationToken ct)
    {
        if (id is null) return NotFound();
        var application = await applications.GetByIdAsync(id.Value, ct);
        if (application is null) return NotFound();
        await FillSelectorsAsync(ct);
        return View("Form", application);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,CandidateId,VacancyId,Stage,AppliedAt,Source,Notes")] JobApplication application, CancellationToken ct)
    {
        if (id != application.Id) return NotFound();
        if (application.CandidateId == 0 || application.VacancyId == 0)
            ModelState.AddModelError("", "Выберите кандидата и вакансию.");

        // Навигационные свойства не приходят из формы: без этого implicit required
        // для non-nullable Candidate/Vacancy блокирует любое сохранение.
        ModelState.Remove(nameof(JobApplication.Candidate));
        ModelState.Remove(nameof(JobApplication.Vacancy));

        if (!ModelState.IsValid)
        {
            await FillSelectorsAsync(ct);
            return View("Form", application);
        }

        try
        {
            await applications.UpdateAsync(application, ct);
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError("", ex.Message);
            await FillSelectorsAsync(ct);
            return View("Form", application);
        }
        catch (DbUpdateException)
        {
            ModelState.AddModelError("", "Не удалось сохранить отклик: возможно, он уже существует.");
            await FillSelectorsAsync(ct);
            return View("Form", application);
        }

        TempData["StatusMessage"] = "Отклик обновлён.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangeStage(int id, ApplicationStage stage, CancellationToken ct)
    {
        try
        {
            await applications.ChangeStageAsync(id, stage, ct);
            TempData["StatusMessage"] = $"Этап изменён на «{StageTitle(stage)}».";
        }
        catch (InvalidOperationException)
        {
            TempData["ErrorMessage"] = "Отклик не найден.";
        }

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int? id, CancellationToken ct)
    {
        if (id is null) return NotFound();
        var application = await applications.GetByIdAsync(id.Value, ct);
        return application is null ? NotFound() : View(application);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken ct)
    {
        try
        {
            await applications.DeleteAsync(id, ct);
            TempData["StatusMessage"] = "Отклик удалён.";
        }
        catch (InvalidOperationException)
        {
            return NotFound();
        }

        return RedirectToAction(nameof(Index));
    }

    internal static string StageTitle(ApplicationStage stage) => stage switch
    {
        ApplicationStage.Applied => "Новые",
        ApplicationStage.Screening => "Скрининг",
        ApplicationStage.Interview => "Собеседования",
        ApplicationStage.Offer => "Оффер",
        ApplicationStage.Hired => "Наняты",
        ApplicationStage.Rejected => "Отказ",
        ApplicationStage.Withdrawn => "Отозваны",
        _ => stage.ToString(),
    };

    private async Task FillSelectorsAsync(CancellationToken ct)
    {
        ViewBag.Candidates = await candidates.GetOptionsAsync(onlyWithResume: true, ct);
        ViewBag.Vacancies = await vacancies.GetOptionsAsync(ct);
    }
}

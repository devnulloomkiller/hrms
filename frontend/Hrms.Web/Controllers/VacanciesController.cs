using Microsoft.AspNetCore.Authorization;
using Hrms.Application.Interfaces;
using Hrms.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Hrms.Web.Controllers;

[Authorize(Roles = "Admin,HR")]
public class VacanciesController(IVacancyService vacancies) : Controller
{
    public async Task<IActionResult> Index(CancellationToken ct)
    {
        return View(await vacancies.GetAllAsync(ct));
    }

    public async Task<IActionResult> Details(int? id, CancellationToken ct)
    {
        if (id is null) return NotFound();
        var vacancy = await vacancies.GetByIdAsync(id.Value, ct);
        return vacancy is null ? NotFound() : View(vacancy);
    }

    public IActionResult Create() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Title,Department,Description,SalaryMin,SalaryMax,Status,PublishedAt,AcceptingResumesUntil")] Vacancy vacancy, CancellationToken ct)
    {
        if (!ModelState.IsValid) return View(vacancy);
        await vacancies.AddAsync(vacancy, ct);
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int? id, CancellationToken ct)
    {
        if (id is null) return NotFound();
        var vacancy = await vacancies.GetByIdAsync(id.Value, ct);
        return vacancy is null ? NotFound() : View(vacancy);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Department,Description,SalaryMin,SalaryMax,Status,PublishedAt,AcceptingResumesUntil")] Vacancy vacancy, CancellationToken ct)
    {
        if (id != vacancy.Id) return NotFound();
        if (!ModelState.IsValid) return View(vacancy);
        try
        {
            await vacancies.UpdateAsync(vacancy, ct);
        }
        catch (InvalidOperationException)
        {
            return NotFound();
        }
        return RedirectToAction(nameof(Index));
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
        }
        catch (InvalidOperationException)
        {
            return NotFound();
        }
        return RedirectToAction(nameof(Index));
    }
}

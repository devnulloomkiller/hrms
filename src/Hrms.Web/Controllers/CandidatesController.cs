using Microsoft.AspNetCore.Authorization;
using Hrms.Application.Interfaces;
using Hrms.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Hrms.Web.Controllers;

[Authorize(Roles = "Admin,HR")]
public class CandidatesController(ICandidateService candidates) : Controller
{
    public async Task<IActionResult> Index(CancellationToken ct) =>
        View(await candidates.GetAllAsync(ct));

    public async Task<IActionResult> Details(int? id, CancellationToken ct)
    {
        if (id is null) return NotFound();
        var candidate = await candidates.GetByIdAsync(id.Value, ct);
        return candidate is null ? NotFound() : View(candidate);
    }

    public IActionResult Create() => View("Form", new Candidate { LastName = "", FirstName = "", Email = "" });

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("LastName,FirstName,Email,Phone")] Candidate candidate, CancellationToken ct)
    {
        if (!ModelState.IsValid) return View("Form", candidate);
        await candidates.AddAsync(candidate, ct);
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int? id, CancellationToken ct)
    {
        if (id is null) return NotFound();
        var candidate = await candidates.GetByIdAsync(id.Value, ct);
        if (candidate is null) return NotFound();
        return View("Form", candidate);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,LastName,FirstName,Email,Phone")] Candidate candidate, CancellationToken ct)
    {
        if (id != candidate.Id) return NotFound();
        if (!ModelState.IsValid) return View("Form", candidate);
        try { await candidates.UpdateAsync(candidate, ct); }
        catch (InvalidOperationException) { return NotFound(); }
        return RedirectToAction(nameof(Index));
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
        try { await candidates.DeleteAsync(id, ct); }
        catch (InvalidOperationException) { return NotFound(); }
        return RedirectToAction(nameof(Index));
    }
}

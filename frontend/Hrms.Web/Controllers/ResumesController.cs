using Hrms.Application.Filters;
using Hrms.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hrms.Web.Controllers;

[Authorize(Roles = "Admin,HR")]
public class ResumesController(IResumeService resumes) : Controller
{
    public async Task<IActionResult> Index(string? search, int? minExperience, string? skill, decimal? maxDesiredSalary, CancellationToken ct)
    {
        ViewBag.Search = search;
        ViewBag.MinExperience = minExperience;
        ViewBag.Skill = skill;
        ViewBag.MaxDesiredSalary = maxDesiredSalary;
        return View(await resumes.SearchAsync(new ResumeFilter(search, minExperience, skill, maxDesiredSalary), ct));
    }

    public async Task<IActionResult> Details(int? id, CancellationToken ct)
    {
        if (id is null) return NotFound();
        var resume = await resumes.GetByIdAsync(id.Value, ct);
        return resume is null ? NotFound() : View(resume);
    }

    public async Task<IActionResult> Edit(int? id, CancellationToken ct)
    {
        if (id is null) return NotFound();
        var resume = await resumes.GetByIdAsync(id.Value, ct);
        if (resume is null) return NotFound();
        return RedirectToAction("Edit", "Candidates", new { id = resume.CandidateId });
    }

    public async Task<IActionResult> Delete(int? id, CancellationToken ct)
    {
        if (id is null) return NotFound();
        var resume = await resumes.GetByIdAsync(id.Value, ct);
        if (resume is null) return NotFound();
        return RedirectToAction("Delete", "Candidates", new { id = resume.CandidateId });
    }

    public IActionResult Create() => RedirectToAction("Create", "Candidates");
}

using Hrms.Application.DTOs;
using Hrms.Application.Filters;
using Hrms.Application.Interfaces;
using Hrms.Domain.Entities;
using Hrms.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Hrms.Application.Services;

public class VacancyService(IHrmsRepository<Vacancy> vacancies) : IVacancyService
{
    public Task<List<Vacancy>> GetAllAsync(VacancyFilter? filter = null, CancellationToken ct = default)
    {
        filter ??= new VacancyFilter();
        var today = DateOnly.FromDateTime(DateTime.Today);
        var query = vacancies.Query()
            .Include(v => v.Applications)
            .Include(v => v.ResumeMatches)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            var term = filter.Search.Trim();
            query = query.Where(v => v.Title.Contains(term) || v.Department.Contains(term));
        }

        if (filter.Status is { } status)
            query = query.Where(v => v.Status == status);

        if (filter.OnlyAccepting)
            query = query.Where(v => v.Status == VacancyStatus.Open
                && (v.AcceptingResumesUntil == null || v.AcceptingResumesUntil >= today));

        return query
            .OrderBy(v => v.Department).ThenBy(v => v.Title)
            .ToListAsync(ct);
    }

    public Task<Vacancy?> GetByIdAsync(int id, CancellationToken ct = default) =>
        vacancies.Query()
            .Include(v => v.Applications).ThenInclude(a => a.Candidate).ThenInclude(c => c.Resume)
            .Include(v => v.Applications).ThenInclude(a => a.Interviews)
            .Include(v => v.ResumeMatches).ThenInclude(m => m.Resume).ThenInclude(r => r.Candidate)
            .FirstOrDefaultAsync(v => v.Id == id, ct);

    public Task AddAsync(Vacancy vacancy, CancellationToken ct = default)
    {
        vacancies.Add(vacancy);
        return vacancies.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(Vacancy vacancy, CancellationToken ct = default)
    {
        var existing = await vacancies.GetByIdAsync(vacancy.Id, ct)
            ?? throw new InvalidOperationException($"Vacancy {vacancy.Id} not found");

        existing.Title = vacancy.Title;
        existing.Department = vacancy.Department;
        existing.Description = vacancy.Description;
        existing.SalaryMin = vacancy.SalaryMin;
        existing.SalaryMax = vacancy.SalaryMax;
        existing.MinExperience = vacancy.MinExperience;
        existing.RequiredSkills = vacancy.RequiredSkills;
        existing.Status = vacancy.Status;
        existing.PublishedAt = vacancy.PublishedAt;
        existing.AcceptingResumesUntil = vacancy.AcceptingResumesUntil;
        await vacancies.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        var existing = await vacancies.Query()
            .Include(v => v.Applications)
            .FirstOrDefaultAsync(v => v.Id == id, ct)
            ?? throw new InvalidOperationException($"Vacancy {id} not found");

        if (existing.Applications.Count > 0)
            throw new InvalidOperationException("По вакансии уже есть отклики. Переведите её в статус «Закрыта» вместо удаления.");

        vacancies.Remove(existing);
        await vacancies.SaveChangesAsync(ct);
    }

    public Task<List<VacancyOptionDto>> GetOptionsAsync(CancellationToken ct = default) =>
        vacancies.Query()
            .OrderBy(v => v.Status).ThenBy(v => v.Title)
            .Select(v => new VacancyOptionDto(v.Id, v.Title, v.Department, v.Status))
            .ToListAsync(ct);
}

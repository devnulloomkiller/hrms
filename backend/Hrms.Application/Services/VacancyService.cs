using Hrms.Application.Interfaces;
using Hrms.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Hrms.Application.Services;

public class VacancyService(IHrmsRepository<Vacancy> vacancies) : IVacancyService
{
    public Task<List<Vacancy>> GetAllAsync(CancellationToken ct = default) =>
        vacancies.Query().OrderBy(v => v.Department).ThenBy(v => v.Title).ToListAsync(ct);

    public Task<Vacancy?> GetByIdAsync(int id, CancellationToken ct = default) =>
        vacancies.Query().FirstOrDefaultAsync(v => v.Id == id, ct);

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
        existing.Status = vacancy.Status;
        existing.PublishedAt = vacancy.PublishedAt;
        existing.AcceptingResumesUntil = vacancy.AcceptingResumesUntil;
        await vacancies.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        var existing = await vacancies.GetByIdAsync(id, ct)
            ?? throw new InvalidOperationException($"Vacancy {id} not found");
        vacancies.Remove(existing);
        await vacancies.SaveChangesAsync(ct);
    }
}

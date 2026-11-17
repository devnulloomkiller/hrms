using Hrms.Application.DTOs;
using Hrms.Application.Filters;
using Hrms.Domain.Entities;

namespace Hrms.Application.Interfaces;

public interface IVacancyService
{
    Task<List<Vacancy>> GetAllAsync(VacancyFilter? filter = null, CancellationToken ct = default);

    Task<Vacancy?> GetByIdAsync(int id, CancellationToken ct = default);

    Task AddAsync(Vacancy vacancy, CancellationToken ct = default);

    Task UpdateAsync(Vacancy vacancy, CancellationToken ct = default);

    Task DeleteAsync(int id, CancellationToken ct = default);

    Task<List<VacancyOptionDto>> GetOptionsAsync(CancellationToken ct = default);
}

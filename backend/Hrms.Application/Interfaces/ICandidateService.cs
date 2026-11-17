using Hrms.Application.DTOs;
using Hrms.Application.Filters;
using Hrms.Domain.Entities;

namespace Hrms.Application.Interfaces;

public interface ICandidateService
{
    Task<List<Candidate>> GetAllAsync(CandidateFilter? filter = null, CancellationToken ct = default);

    Task<Candidate?> GetByIdAsync(int id, CancellationToken ct = default);

    Task AddAsync(Candidate candidate, CancellationToken ct = default);

    Task UpdateAsync(Candidate candidate, CancellationToken ct = default);

    Task DeleteAsync(int id, CancellationToken ct = default);

    Task<List<CandidateOptionDto>> GetOptionsAsync(bool onlyWithResume = true, CancellationToken ct = default);
}

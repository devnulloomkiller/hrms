using Hrms.Domain.Entities;

namespace Hrms.Application.Interfaces;

public interface IInterviewService
{
    Task<List<Interview>> GetAllAsync(CancellationToken ct = default);

    Task<Interview?> GetByIdAsync(int id, CancellationToken ct = default);

    Task AddAsync(Interview interview, CancellationToken ct = default);

    Task UpdateAsync(Interview interview, CancellationToken ct = default);

    Task DeleteAsync(int id, CancellationToken ct = default);

    Task<List<Interviewer>> GetInterviewersAsync(CancellationToken ct = default);
}

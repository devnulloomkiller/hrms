using Hrms.Domain.Entities;

namespace Hrms.Application.Interfaces;

public interface IResumeService
{
    Task<List<Resume>> GetAllAsync(CancellationToken ct = default);

    Task<Resume?> GetByIdAsync(int id, CancellationToken ct = default);

    Task AddAsync(Resume resume, CancellationToken ct = default);

    Task UpdateAsync(Resume resume, CancellationToken ct = default);

    Task DeleteAsync(int id, CancellationToken ct = default);
}

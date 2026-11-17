using Hrms.Application.DTOs;
using Hrms.Application.Filters;
using Hrms.Domain.Entities;
using Hrms.Domain.Enums;

namespace Hrms.Application.Interfaces;

public interface IJobApplicationService
{
    Task<List<JobApplication>> GetAllAsync(ApplicationFilter? filter = null, CancellationToken ct = default);

    Task<JobApplication?> GetByIdAsync(int id, CancellationToken ct = default);

    Task AddAsync(JobApplication application, CancellationToken ct = default);

    Task UpdateAsync(JobApplication application, CancellationToken ct = default);

    Task DeleteAsync(int id, CancellationToken ct = default);

    Task ChangeStageAsync(int id, ApplicationStage stage, CancellationToken ct = default);

    Task<List<ApplicationOptionDto>> GetOptionsAsync(CancellationToken ct = default);
}

using Hrms.Application.DTOs;
using Hrms.Application.Filters;
using Hrms.Domain.Entities;
using Hrms.Domain.Enums;

namespace Hrms.Application.Interfaces;

public interface IResumeService
{
    Task<List<Resume>> SearchAsync(ResumeFilter? filter = null, CancellationToken ct = default);

    Task<Resume?> GetByIdAsync(int id, CancellationToken ct = default);

    Task<Resume?> GetByCandidateIdAsync(int candidateId, CancellationToken ct = default);

    Task<List<ResumeMatchDto>> FindResumesForVacancyAsync(int vacancyId, ResumeMatchFilter? filter = null, CancellationToken ct = default);

    Task<List<VacancyMatchDto>> FindVacanciesForResumeAsync(int resumeId, VacancyMatchFilter? filter = null, CancellationToken ct = default);

    Task<int> LinkResumesToVacancyAsync(int vacancyId, IEnumerable<int> resumeIds, MatchStatus status, CancellationToken ct = default);

    Task<int> LinkVacanciesToResumeAsync(int resumeId, IEnumerable<int> vacancyIds, MatchStatus status, CancellationToken ct = default);

    Task UpdateMatchStatusAsync(int vacancyId, int resumeId, MatchStatus status, CancellationToken ct = default);

    Task UnlinkAsync(int vacancyId, int resumeId, CancellationToken ct = default);
}

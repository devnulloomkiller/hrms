using Hrms.Application.DTOs;
using Hrms.Application.Interfaces;
using Hrms.Domain.Entities;
using Hrms.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Hrms.Application.Services;

public class ReportService(
    IHrmsRepository<Vacancy> vacancies,
    IHrmsRepository<Candidate> candidates) : IReportService
{
    /// <summary>
    /// Открытые вакансии с дедлайном в ближайшие N дней: считаем отклики и подборы,
    /// чтобы HR видел, где заканчивается поток кандидатов.
    /// </summary>
    public async Task<List<ExpiringVacancyDto>> GetVacanciesWithExpiringDeadlineAsync(int days, CancellationToken ct = default)
    {
        var today = DateOnly.FromDateTime(DateTime.Today);
        var limit = today.AddDays(days);

        return (await vacancies.Query()
            .Where(v => v.Status == VacancyStatus.Open
                     && v.AcceptingResumesUntil != null
                     && v.AcceptingResumesUntil >= today
                     && v.AcceptingResumesUntil <= limit)
            .Select(v => new ExpiringVacancyDto(
                v.Id,
                v.Title,
                v.Department,
                v.AcceptingResumesUntil!.Value,
                v.AcceptingResumesUntil.Value.DayNumber - today.DayNumber,
                v.Applications.Count,
                v.ResumeMatches.Count))
            .ToListAsync(ct))
            .OrderBy(dto => dto.AcceptingResumesUntil)
            .ToList();
    }

    /// <summary>
    /// Карточка кандидата: одно резюме, все отклики с собеседованиями и сохранённые подборы.
    /// </summary>
    public async Task<CandidateProfileDto?> GetCandidateProfileAsync(int candidateId, CancellationToken ct = default)
    {
        var candidate = await candidates.Query()
            .Include(c => c.Resume).ThenInclude(r => r!.Details)
            .Include(c => c.Applications).ThenInclude(a => a.Vacancy)
            .Include(c => c.Applications).ThenInclude(a => a.Interviews)
            .Include(c => c.Resume!).ThenInclude(r => r!.VacancyMatches).ThenInclude(m => m.Vacancy)
            .FirstOrDefaultAsync(c => c.Id == candidateId, ct);

        if (candidate is null) return null;

        var applications = candidate.Applications
            .OrderByDescending(a => a.AppliedAt)
            .Select(a => new CandidateApplicationDto(
                a.Id,
                a.VacancyId,
                a.Vacancy.Title,
                a.Vacancy.Department,
                a.Stage,
                a.AppliedAt,
                a.Interviews.Count))
            .ToList();

        var matches = candidate.Resume?.VacancyMatches
            .OrderByDescending(m => m.MatchScore)
            .Select(m => new CandidateVacancyMatchDto(
                m.VacancyId,
                m.Vacancy.Title,
                m.Vacancy.Department,
                m.Vacancy.Status,
                m.Status,
                m.MatchScore))
            .ToList() ?? [];

        return new CandidateProfileDto(
            candidate.Id,
            $"{candidate.LastName} {candidate.FirstName}",
            candidate.Email,
            candidate.Phone,
            candidate.Resume?.Headline,
            candidate.Resume?.ExperienceYears ?? 0,
            candidate.Resume?.DesiredSalary,
            candidate.Resume?.Details?.Skills,
            candidate.Resume?.Details?.Education,
            candidate.Resume?.Summary,
            applications.Count,
            applications.Sum(a => a.InterviewCount),
            applications,
            matches);
    }

    /// <summary>
    /// Собеседования по вакансиям за период с разбиением по результатам.
    /// </summary>
    public async Task<List<VacancyInterviewSummaryDto>> GetInterviewSummaryByVacancyAsync(DateTime from, DateTime to, CancellationToken ct = default)
    {
        var list = await vacancies.Query()
            .Include(v => v.Applications).ThenInclude(a => a.Interviews)
            .AsNoTracking()
            .ToListAsync(ct);

        return list
            .Select(v =>
            {
                var interviews = v.Applications
                    .SelectMany(a => a.Interviews)
                    .Where(i => i.ScheduledAt >= from && i.ScheduledAt < to)
                    .ToList();

                return new VacancyInterviewSummaryDto(
                    v.Id,
                    v.Title,
                    v.Department,
                    interviews.Count,
                    interviews.Count(i => i.Result == InterviewResult.Passed),
                    interviews.Count(i => i.Result == InterviewResult.Failed),
                    interviews.Count(i => i.Result == InterviewResult.Scheduled),
                    interviews.Count(i => i.Result == InterviewResult.Cancelled));
            })
            .Where(x => x.TotalInterviews > 0)
            .OrderByDescending(x => x.TotalInterviews)
            .ToList();
    }

    /// <summary>
    /// Воронка по вакансиям: условная агрегация откликов по этапам и конверсия в найм.
    /// </summary>
    public async Task<List<VacancyPipelineDto>> GetPipelineByVacancyAsync(CancellationToken ct = default)
    {
        var list = await vacancies.Query()
            .Include(v => v.Applications)
            .AsNoTracking()
            .ToListAsync(ct);

        return list
            .Select(v => new VacancyPipelineDto(
                v.Id,
                v.Title,
                v.Department,
                v.Status,
                v.Applications.Count,
                v.Applications.Count(a => a.Stage == ApplicationStage.Applied),
                v.Applications.Count(a => a.Stage == ApplicationStage.Screening),
                v.Applications.Count(a => a.Stage == ApplicationStage.Interview),
                v.Applications.Count(a => a.Stage == ApplicationStage.Offer),
                v.Applications.Count(a => a.Stage == ApplicationStage.Hired),
                v.Applications.Count(a => a.Stage == ApplicationStage.Rejected),
                v.Applications.Count(a => a.Stage == ApplicationStage.Withdrawn)))
            .OrderByDescending(x => x.TotalApplications)
            .ThenBy(x => x.Title)
            .ToList();
    }
}

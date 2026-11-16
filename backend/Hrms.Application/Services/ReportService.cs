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
    /// Запрос 1 (нетривиальный): открытые вакансии, у которых срок приёма резюме истекает
    /// в ближайшие N дней, с подсчётом поданных резюме и числом дней до дедлайна.
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
                v.Interviews.Select(i => i.ResumeId).Distinct().Count()))
            .ToListAsync(ct))
            .OrderBy(dto => dto.AcceptingResumesUntil)
            .ToList();
    }

    /// <summary>
    /// Запрос 2 (нетривиальный): агрегированный профиль кандидата — все его резюме
    /// с вакансиями и все собеседования с участниками (N:N), в одном проходе.
    /// </summary>
    public async Task<CandidateProfileDto?> GetCandidateProfileAsync(int candidateId, CancellationToken ct = default)
    {
        var candidate = await candidates.Query()
            .Where(c => c.Id == candidateId)
            .Select(c => new
            {
                c.Id,
                c.LastName,
                c.FirstName,
                c.Email,
                c.Phone,
                Resumes = c.Resumes
                    .OrderByDescending(r => r.SubmittedAt)
                    .Select(r => new CandidateResumeDto(
                        r.Id,
                        r.Vacancy != null ? r.Vacancy.Title : null,
                        r.Status,
                        r.ExperienceYears,
                        DateOnly.FromDateTime(r.SubmittedAt),
                        r.Interviews
                            .OrderBy(i => i.ScheduledAt)
                            .Select(i => new InterviewInfoDto(
                                i.ScheduledAt,
                                i.Mode,
                                i.Result,
                                i.Vacancy.Title,
                                i.Participants.Select(p => p.Interviewer.FullName + " (" + p.Role + ")").ToList()))
                            .ToList()))
                    .ToList(),
            })
            .FirstOrDefaultAsync(ct);

        if (candidate is null) return null;

        return new CandidateProfileDto(
            candidate.Id,
            $"{candidate.LastName} {candidate.FirstName}",
            candidate.Email,
            candidate.Phone,
            candidate.Resumes.Count,
            candidate.Resumes.Sum(r => r.Interviews.Count),
            candidate.Resumes);
    }

    /// <summary>
    /// Запрос 3 (нетривиальный): количество собеседований по каждой вакансии за период
    /// с разбиением по результатам — условная агрегация внутри группировки.
    /// </summary>
    public async Task<List<VacancyInterviewSummaryDto>> GetInterviewSummaryByVacancyAsync(DateTime from, DateTime to, CancellationToken ct = default)
    {
        return (await vacancies.Query()
            .Select(v => new
            {
                v.Id,
                v.Title,
                v.Department,
                Interviews = v.Interviews.Where(i => i.ScheduledAt >= from && i.ScheduledAt < to),
            })
            .Where(x => x.Interviews.Any())
            .Select(x => new VacancyInterviewSummaryDto(
                x.Id,
                x.Title,
                x.Department,
                x.Interviews.Count(),
                x.Interviews.Count(i => i.Result == InterviewResult.Passed),
                x.Interviews.Count(i => i.Result == InterviewResult.Failed),
                x.Interviews.Count(i => i.Result == InterviewResult.Scheduled),
                x.Interviews.Count(i => i.Result == InterviewResult.Cancelled)))
            .ToListAsync(ct))
            .OrderByDescending(x => x.TotalInterviews)
            .ToList();
    }
}

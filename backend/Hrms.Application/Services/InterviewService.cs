using Hrms.Application.DTOs;
using Hrms.Application.Filters;
using Hrms.Application.Interfaces;
using Hrms.Domain.Entities;
using Hrms.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Hrms.Application.Services;

public class InterviewService(
    IHrmsRepository<Interview> interviews,
    IHrmsRepository<Interviewer> interviewers,
    IHrmsRepository<JobApplication> applications) : IInterviewService
{
    public Task<List<Interview>> GetAllAsync(InterviewFilter? filter = null, CancellationToken ct = default)
    {
        filter ??= new InterviewFilter();
        var query = interviews.Query()
            .Include(i => i.Application).ThenInclude(a => a.Candidate)
            .Include(i => i.Application).ThenInclude(a => a.Vacancy)
            .Include(i => i.Participants).ThenInclude(p => p.Interviewer)
            .AsQueryable();

        if (filter.Upcoming is true)
            query = query.Where(i => i.ScheduledAt >= DateTime.Now);
        else if (filter.Upcoming is false)
            query = query.Where(i => i.ScheduledAt < DateTime.Now);

        if (filter.Result is { } result)
            query = query.Where(i => i.Result == result);

        if (filter.VacancyId is { } vacancyId)
            query = query.Where(i => i.Application.VacancyId == vacancyId);

        return query.OrderBy(i => i.ScheduledAt).ToListAsync(ct);
    }

    public Task<Interview?> GetByIdAsync(int id, CancellationToken ct = default) =>
        interviews.Query()
            .Include(i => i.Application).ThenInclude(a => a.Candidate)
            .Include(i => i.Application).ThenInclude(a => a.Vacancy)
            .Include(i => i.Participants).ThenInclude(p => p.Interviewer)
            .FirstOrDefaultAsync(i => i.Id == id, ct);

    public async Task AddAsync(Interview interview, CancellationToken ct = default)
    {
        var application = await applications.Query()
            .Include(a => a.Candidate)
            .Include(a => a.Vacancy)
            .FirstOrDefaultAsync(a => a.Id == interview.ApplicationId, ct)
            ?? throw new InvalidOperationException("Отклик не найден.");

        interview.Application = application;
        interviews.Add(interview);

        if (application.Stage is ApplicationStage.Applied or ApplicationStage.Screening)
        {
            application.Stage = ApplicationStage.Interview;
            application.UpdatedAt = DateTime.Now;
        }

        await interviews.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(Interview interview, CancellationToken ct = default)
    {
        var existing = await interviews.Query()
            .Include(i => i.Participants)
            .FirstOrDefaultAsync(i => i.Id == interview.Id, ct)
            ?? throw new InvalidOperationException($"Interview {interview.Id} not found");

        existing.ApplicationId = interview.ApplicationId;
        existing.ScheduledAt = interview.ScheduledAt;
        existing.Mode = interview.Mode;
        existing.Result = interview.Result;
        existing.Notes = interview.Notes;

        existing.Participants.Clear();
        foreach (var participant in interview.Participants)
            existing.Participants.Add(new InterviewInterviewer
            {
                InterviewId = existing.Id,
                InterviewerId = participant.InterviewerId,
                Role = participant.Role,
            });

        await interviews.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        var existing = await interviews.GetByIdAsync(id, ct)
            ?? throw new InvalidOperationException($"Interview {id} not found");
        interviews.Remove(existing);
        await interviews.SaveChangesAsync(ct);
    }

    public Task<List<Interviewer>> GetInterviewersAsync(CancellationToken ct = default) =>
        interviewers.Query().OrderBy(i => i.FullName).ToListAsync(ct);

    public Task<List<ApplicationOptionDto>> GetApplicationOptionsAsync(CancellationToken ct = default) =>
        applications.Query()
            .Include(a => a.Candidate)
            .Include(a => a.Vacancy)
            .OrderByDescending(a => a.AppliedAt)
            .Select(a => new ApplicationOptionDto(
                a.Id,
                a.Candidate.LastName + " " + a.Candidate.FirstName,
                a.Vacancy.Title,
                a.Stage))
            .ToListAsync(ct);
}

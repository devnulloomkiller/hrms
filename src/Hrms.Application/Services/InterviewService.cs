using Hrms.Application.Interfaces;
using Hrms.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Hrms.Application.Services;

public class InterviewService(
    IHrmsRepository<Interview> interviews,
    IHrmsRepository<Interviewer> interviewers) : IInterviewService
{
    public Task<List<Interview>> GetAllAsync(CancellationToken ct = default) =>
        interviews.Query()
            .Include(i => i.Resume!).ThenInclude(r => r.Candidate)
            .Include(i => i.Vacancy)
            .Include(i => i.Participants).ThenInclude(p => p.Interviewer)
            .OrderBy(i => i.ScheduledAt)
            .ToListAsync(ct);

    public Task<Interview?> GetByIdAsync(int id, CancellationToken ct = default) =>
        interviews.Query()
            .Include(i => i.Resume!).ThenInclude(r => r.Candidate)
            .Include(i => i.Vacancy)
            .Include(i => i.Participants).ThenInclude(p => p.Interviewer)
            .FirstOrDefaultAsync(i => i.Id == id, ct);

    public Task AddAsync(Interview interview, CancellationToken ct = default)
    {
        interviews.Add(interview);
        return interviews.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(Interview interview, CancellationToken ct = default)
    {
        var existing = await interviews.Query()
            .Include(i => i.Participants)
            .FirstOrDefaultAsync(i => i.Id == interview.Id, ct)
            ?? throw new InvalidOperationException($"Interview {interview.Id} not found");
        existing.ResumeId = interview.ResumeId;
        existing.VacancyId = interview.VacancyId;
        existing.ScheduledAt = interview.ScheduledAt;
        existing.Mode = interview.Mode;
        existing.Result = interview.Result;
        existing.Notes = interview.Notes;
        existing.Participants.Clear();
        foreach (var p in interview.Participants)
            existing.Participants.Add(new InterviewInterviewer { InterviewId = existing.Id, InterviewerId = p.InterviewerId, Role = p.Role });
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
}

using Hrms.Application.Interfaces;
using Hrms.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Hrms.Application.Services;

public class ResumeService(IHrmsRepository<Resume> resumes) : IResumeService
{
    public Task<List<Resume>> GetAllAsync(CancellationToken ct = default) =>
        resumes.Query()
            .Include(r => r.Candidate)
            .Include(r => r.Vacancy)
            .OrderByDescending(r => r.SubmittedAt)
            .ToListAsync(ct)!;

    public Task<Resume?> GetByIdAsync(int id, CancellationToken ct = default) =>
        resumes.Query()
            .Include(r => r.Candidate)
            .Include(r => r.Vacancy)
            .Include(r => r.Details)
            .FirstOrDefaultAsync(r => r.Id == id, ct);

    public Task AddAsync(Resume resume, CancellationToken ct = default)
    {
        resumes.Add(resume);
        return resumes.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(Resume resume, CancellationToken ct = default)
    {
        var existing = await resumes.Query().FirstOrDefaultAsync(r => r.Id == resume.Id, ct)
            ?? throw new InvalidOperationException($"Resume {resume.Id} not found");
        existing.CandidateId = resume.CandidateId;
        existing.VacancyId = resume.VacancyId;
        existing.Summary = resume.Summary;
        existing.ExperienceYears = resume.ExperienceYears;
        existing.SubmittedAt = resume.SubmittedAt;
        existing.Status = resume.Status;
        await resumes.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        var existing = await resumes.GetByIdAsync(id, ct)
            ?? throw new InvalidOperationException($"Resume {id} not found");
        resumes.Remove(existing);
        await resumes.SaveChangesAsync(ct);
    }
}

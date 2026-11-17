using Hrms.Application.DTOs;
using Hrms.Application.Filters;
using Hrms.Application.Interfaces;
using Hrms.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Hrms.Application.Services;

public class CandidateService(IHrmsRepository<Candidate> candidates) : ICandidateService
{
    public async Task<List<Candidate>> GetAllAsync(CandidateFilter? filter = null, CancellationToken ct = default)
    {
        filter ??= new CandidateFilter();
        var query = candidates.Query()
            .Include(c => c.Resume!).ThenInclude(r => r.Details)
            .Include(c => c.Applications).ThenInclude(a => a.Vacancy)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            var term = filter.Search.Trim();
            query = query.Where(c =>
                c.LastName.Contains(term) ||
                c.FirstName.Contains(term) ||
                c.Email.Contains(term) ||
                (c.Resume != null && c.Resume.Headline.Contains(term)));
        }

        if (filter.MinExperience is { } minExperience)
            query = query.Where(c => c.Resume != null && c.Resume.ExperienceYears >= minExperience);

        if (!string.IsNullOrWhiteSpace(filter.Skill))
        {
            var skill = filter.Skill.Trim();
            query = query.Where(c =>
                c.Resume != null &&
                c.Resume.Details != null &&
                c.Resume.Details.Skills != null &&
                c.Resume.Details.Skills.Contains(skill));
        }

        if (filter.OnlyWithResume)
            query = query.Where(c => c.Resume != null);

        return await query
            .OrderBy(c => c.LastName).ThenBy(c => c.FirstName)
            .ToListAsync(ct);
    }

    public Task<Candidate?> GetByIdAsync(int id, CancellationToken ct = default) =>
        candidates.Query()
            .Include(c => c.Resume!).ThenInclude(r => r.Details)
            .Include(c => c.Resume!).ThenInclude(r => r.VacancyMatches).ThenInclude(m => m.Vacancy)
            .Include(c => c.Applications).ThenInclude(a => a.Vacancy)
            .Include(c => c.Applications).ThenInclude(a => a.Interviews)
            .FirstOrDefaultAsync(c => c.Id == id, ct);

    public Task AddAsync(Candidate candidate, CancellationToken ct = default)
    {
        if (candidate.Resume is not null)
        {
            candidate.Resume.CreatedAt = DateTime.Now;
            candidate.Resume.UpdatedAt = DateTime.Now;
        }
        candidates.Add(candidate);
        return candidates.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(Candidate candidate, CancellationToken ct = default)
    {
        var existing = await candidates.Query()
            .Include(c => c.Resume).ThenInclude(r => r!.Details)
            .FirstOrDefaultAsync(c => c.Id == candidate.Id, ct)
            ?? throw new InvalidOperationException($"Candidate {candidate.Id} not found");

        existing.LastName = candidate.LastName;
        existing.FirstName = candidate.FirstName;
        existing.Email = candidate.Email;
        existing.Phone = candidate.Phone;

        if (candidate.Resume is not null)
        {
            existing.Resume ??= new Resume
            {
                CandidateId = existing.Id,
                Headline = candidate.Resume.Headline,
                Summary = candidate.Resume.Summary,
                CreatedAt = DateTime.Now,
            };

            existing.Resume.Headline = candidate.Resume.Headline;
            existing.Resume.Summary = candidate.Resume.Summary;
            existing.Resume.ExperienceYears = candidate.Resume.ExperienceYears;
            existing.Resume.DesiredSalary = candidate.Resume.DesiredSalary;
            existing.Resume.UpdatedAt = DateTime.Now;

            if (candidate.Resume.Details is not null)
            {
                existing.Resume.Details ??= new ResumeDetails();
                existing.Resume.Details.Education = candidate.Resume.Details.Education;
                existing.Resume.Details.Skills = candidate.Resume.Details.Skills;
                existing.Resume.Details.FilePath = candidate.Resume.Details.FilePath;
            }
        }

        await candidates.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        var existing = await candidates.GetByIdAsync(id, ct)
            ?? throw new InvalidOperationException($"Candidate {id} not found");
        candidates.Remove(existing);
        await candidates.SaveChangesAsync(ct);
    }

    public Task<List<CandidateOptionDto>> GetOptionsAsync(bool onlyWithResume = true, CancellationToken ct = default)
    {
        IQueryable<Candidate> query = candidates.Query().Include(c => c.Resume);
        if (onlyWithResume) query = query.Where(c => c.Resume != null);

        return query
            .OrderBy(c => c.LastName).ThenBy(c => c.FirstName)
            .Select(c => new CandidateOptionDto(
                c.Id,
                c.LastName + " " + c.FirstName,
                c.Email,
                c.Resume != null ? c.Resume.Headline : "Резюме не заполнено",
                c.Resume != null ? c.Resume.ExperienceYears : 0,
                c.Resume != null))
            .ToListAsync(ct);
    }
}

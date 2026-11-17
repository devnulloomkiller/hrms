using Hrms.Application.DTOs;
using Hrms.Application.Filters;
using Hrms.Application.Interfaces;
using Hrms.Domain.Entities;
using Hrms.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Hrms.Application.Services;

public class ResumeService(
    IHrmsRepository<Resume> resumes,
    IHrmsRepository<Vacancy> vacancies,
    IHrmsRepository<ResumeVacancyMatch> matches) : IResumeService
{
    public Task<List<Resume>> SearchAsync(ResumeFilter? filter = null, CancellationToken ct = default)
    {
        filter ??= new ResumeFilter();
        var query = resumes.Query()
            .Include(r => r.Candidate).ThenInclude(c => c.Applications).ThenInclude(a => a.Vacancy)
            .Include(r => r.Details)
            .Include(r => r.VacancyMatches)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            var term = filter.Search.Trim();
            query = query.Where(r =>
                r.Headline.Contains(term) ||
                r.Summary.Contains(term) ||
                r.Candidate.LastName.Contains(term) ||
                r.Candidate.FirstName.Contains(term) ||
                (r.Details != null && r.Details.Skills != null && r.Details.Skills.Contains(term)));
        }

        if (filter.MinExperience is { } minExperience)
            query = query.Where(r => r.ExperienceYears >= minExperience);

        if (!string.IsNullOrWhiteSpace(filter.Skill))
        {
            var skill = filter.Skill.Trim();
            query = query.Where(r => r.Details != null && r.Details.Skills != null && r.Details.Skills.Contains(skill));
        }

        if (filter.MaxDesiredSalary is { } maxSalary)
            query = query.Where(r => r.DesiredSalary == null || r.DesiredSalary <= maxSalary);

        return query
            .OrderBy(r => r.Candidate.LastName).ThenBy(r => r.Candidate.FirstName)
            .ToListAsync(ct);
    }

    public Task<Resume?> GetByIdAsync(int id, CancellationToken ct = default) =>
        resumes.Query()
            .Include(r => r.Candidate).ThenInclude(c => c.Applications).ThenInclude(a => a.Vacancy)
            .Include(r => r.Candidate).ThenInclude(c => c.Applications).ThenInclude(a => a.Interviews)
            .Include(r => r.Details)
            .Include(r => r.VacancyMatches).ThenInclude(m => m.Vacancy)
            .FirstOrDefaultAsync(r => r.Id == id, ct);

    public Task<Resume?> GetByCandidateIdAsync(int candidateId, CancellationToken ct = default) =>
        resumes.Query()
            .Include(r => r.Candidate).ThenInclude(c => c.Applications).ThenInclude(a => a.Vacancy)
            .Include(r => r.Details)
            .Include(r => r.VacancyMatches).ThenInclude(m => m.Vacancy)
            .FirstOrDefaultAsync(r => r.CandidateId == candidateId, ct);

    public async Task<List<ResumeMatchDto>> FindResumesForVacancyAsync(int vacancyId, ResumeMatchFilter? filter = null, CancellationToken ct = default)
    {
        filter ??= new ResumeMatchFilter();
        var vacancy = await vacancies.Query()
            .Include(v => v.ResumeMatches)
            .Include(v => v.Applications)
            .FirstOrDefaultAsync(v => v.Id == vacancyId, ct);

        if (vacancy is null) return [];
        if (filter.OnlyOpenVacancy && vacancy.Status is not (VacancyStatus.Open or VacancyStatus.Paused)) return [];

        var query = resumes.Query()
            .Include(r => r.Candidate)
            .Include(r => r.Details)
            .Include(r => r.VacancyMatches.Where(m => m.VacancyId == vacancyId))
            .Include(r => r.Candidate.Applications.Where(a => a.VacancyId == vacancyId))
            .AsQueryable();

        if (filter.MinExperience is { } minExperience)
            query = query.Where(r => r.ExperienceYears >= minExperience);

        if (!string.IsNullOrWhiteSpace(filter.Skill))
        {
            var skill = filter.Skill.Trim();
            query = query.Where(r => r.Details != null && r.Details.Skills != null && r.Details.Skills.Contains(skill));
        }

        if (filter.MaxDesiredSalary is { } maxSalary)
            query = query.Where(r => r.DesiredSalary == null || r.DesiredSalary <= maxSalary);

        if (filter.OnlyNotLinked)
            query = query.Where(r =>
                !r.VacancyMatches.Any(m => m.VacancyId == vacancyId) &&
                !r.Candidate.Applications.Any(a => a.VacancyId == vacancyId));

        var candidates = await query.ToListAsync(ct);

        return candidates
            .Select(r =>
            {
                var link = r.VacancyMatches.FirstOrDefault();
                var application = r.Candidate.Applications.FirstOrDefault();
                return new ResumeMatchDto(
                    r.Id,
                    r.CandidateId,
                    $"{r.Candidate.LastName} {r.Candidate.FirstName}",
                    r.Headline,
                    r.ExperienceYears,
                    r.DesiredSalary,
                    r.Details?.Skills,
                    MatchScoring.Score(r, vacancy),
                    link is not null || application is not null,
                    link?.Status,
                    application?.Stage);
            })
            .OrderByDescending(x => x.MatchScore)
            .ThenBy(x => x.CandidateName)
            .ToList();
    }

    public async Task<List<VacancyMatchDto>> FindVacanciesForResumeAsync(int resumeId, VacancyMatchFilter? filter = null, CancellationToken ct = default)
    {
        filter ??= new VacancyMatchFilter();
        var resume = await resumes.Query()
            .Include(r => r.Details)
            .FirstOrDefaultAsync(r => r.Id == resumeId, ct);

        if (resume is null) return [];

        var today = DateOnly.FromDateTime(DateTime.Today);
        var query = vacancies.Query()
            .Include(v => v.Applications.Where(a => a.CandidateId == resume.CandidateId))
            .Include(v => v.ResumeMatches.Where(m => m.ResumeId == resumeId))
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            var term = filter.Search.Trim();
            query = query.Where(v => v.Title.Contains(term) || v.Department.Contains(term));
        }

        if (filter.OnlyOpen)
            query = query.Where(v => v.Status == VacancyStatus.Open);

        if (filter.OnlyAccepting)
            query = query.Where(v => v.Status == VacancyStatus.Open
                && (v.AcceptingResumesUntil == null || v.AcceptingResumesUntil >= today));

        if (filter.OnlyNotLinked)
            query = query.Where(v =>
                !v.Applications.Any(a => a.CandidateId == resume.CandidateId) &&
                !v.ResumeMatches.Any(m => m.ResumeId == resumeId));

        var list = await query.OrderBy(v => v.Title).ToListAsync(ct);

        return list
            .Select(v =>
            {
                var link = v.ResumeMatches.FirstOrDefault();
                var application = v.Applications.FirstOrDefault();
                return new VacancyMatchDto(
                    v.Id,
                    v.Title,
                    v.Department,
                    v.Status,
                    v.AcceptingResumesUntil,
                    v.SalaryMin,
                    v.SalaryMax,
                    v.MinExperience,
                    v.RequiredSkills,
                    MatchScoring.Score(resume, v),
                    link is not null || application is not null,
                    link?.Status,
                    application?.Stage);
            })
            .OrderByDescending(x => x.MatchScore)
            .ThenBy(x => x.Title)
            .ToList();
    }

    public async Task<int> LinkResumesToVacancyAsync(int vacancyId, IEnumerable<int> resumeIds, MatchStatus status, CancellationToken ct = default)
    {
        var vacancy = await vacancies.Query()
            .Include(v => v.ResumeMatches)
            .FirstOrDefaultAsync(v => v.Id == vacancyId, ct)
            ?? throw new InvalidOperationException("Вакансия не найдена.");

        var ids = resumeIds.Distinct().ToArray();
        if (ids.Length == 0) return 0;

        var selected = await resumes.Query()
            .Include(r => r.Details)
            .Where(r => ids.Contains(r.Id))
            .ToListAsync(ct);

        var added = 0;
        foreach (var resume in selected)
        {
            var existing = vacancy.ResumeMatches.FirstOrDefault(m => m.ResumeId == resume.Id);
            if (existing is not null)
            {
                existing.Status = status;
                existing.MatchScore = MatchScoring.Score(resume, vacancy);
                existing.MatchedAt = DateTime.Now;
                continue;
            }

            matches.Add(new ResumeVacancyMatch
            {
                ResumeId = resume.Id,
                VacancyId = vacancy.Id,
                Status = status,
                MatchScore = MatchScoring.Score(resume, vacancy),
                MatchedAt = DateTime.Now,
            });
            added++;
        }

        await matches.SaveChangesAsync(ct);
        return added;
    }

    public async Task<int> LinkVacanciesToResumeAsync(int resumeId, IEnumerable<int> vacancyIds, MatchStatus status, CancellationToken ct = default)
    {
        var resume = await resumes.Query()
            .Include(r => r.Details)
            .FirstOrDefaultAsync(r => r.Id == resumeId, ct)
            ?? throw new InvalidOperationException("Резюме не найдено.");

        var ids = vacancyIds.Distinct().ToArray();
        if (ids.Length == 0) return 0;

        var selected = await vacancies.Query()
            .Where(v => ids.Contains(v.Id))
            .ToListAsync(ct);

        var existingIds = await matches.Query()
            .Where(m => m.ResumeId == resumeId)
            .Select(m => m.VacancyId)
            .ToListAsync(ct);

        var added = 0;
        foreach (var vacancy in selected)
        {
            if (existingIds.Contains(vacancy.Id)) continue;

            matches.Add(new ResumeVacancyMatch
            {
                ResumeId = resume.Id,
                VacancyId = vacancy.Id,
                Status = status,
                MatchScore = MatchScoring.Score(resume, vacancy),
                MatchedAt = DateTime.Now,
            });
            added++;
        }

        await matches.SaveChangesAsync(ct);
        return added;
    }

    public async Task UpdateMatchStatusAsync(int vacancyId, int resumeId, MatchStatus status, CancellationToken ct = default)
    {
        var match = await matches.Query()
            .FirstOrDefaultAsync(m => m.VacancyId == vacancyId && m.ResumeId == resumeId, ct)
            ?? throw new InvalidOperationException("Подбор не найден.");
        match.Status = status;
        await matches.SaveChangesAsync(ct);
    }

    public async Task UnlinkAsync(int vacancyId, int resumeId, CancellationToken ct = default)
    {
        var match = await matches.Query()
            .FirstOrDefaultAsync(m => m.VacancyId == vacancyId && m.ResumeId == resumeId, ct);
        if (match is null) return;
        matches.Remove(match);
        await matches.SaveChangesAsync(ct);
    }
}

internal static class MatchScoring
{
    /// <summary>
    /// 40 баллов за опыт, 40 за совпадение ключевых навыков и 20 за попадание в зарплатную вилку.
    /// Недостаток опыта и навыков масштабируется пропорционально, а не обнуляет оценку.
    /// </summary>
    public static int Score(Resume resume, Vacancy vacancy)
    {
        var experienceScore = vacancy.MinExperience <= 0
            ? 40
            : (int)Math.Round(Math.Clamp((double)resume.ExperienceYears / vacancy.MinExperience, 0, 1) * 40);

        var requiredSkills = SplitSkills(vacancy.RequiredSkills);
        var skillScore = 40;
        if (requiredSkills.Count > 0)
        {
            var resumeSkills = SplitSkills(resume.Details?.Skills);
            var matched = requiredSkills.Count(required =>
                resumeSkills.Any(skill => SkillMatches(skill, required)));
            skillScore = (int)Math.Round((double)matched / requiredSkills.Count * 40);
        }

        var salaryScore = 20;
        if (vacancy.SalaryMax is { } max && resume.DesiredSalary is { } desired && desired > max)
            salaryScore = (int)Math.Round(Math.Clamp((double)(max / desired), 0, 1) * 20);

        return Math.Clamp(experienceScore + skillScore + salaryScore, 0, 100);
    }

    private static bool SkillMatches(string resumeSkill, string requiredSkill) =>
        resumeSkill.Contains(requiredSkill, StringComparison.OrdinalIgnoreCase) ||
        requiredSkill.Contains(resumeSkill, StringComparison.OrdinalIgnoreCase);

    private static List<string> SplitSkills(string? value) =>
        string.IsNullOrWhiteSpace(value)
            ? []
            : value.Split([',', ';', '/', '\n'], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();
}

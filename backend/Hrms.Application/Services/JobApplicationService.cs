using Hrms.Application.DTOs;
using Hrms.Application.Filters;
using Hrms.Application.Interfaces;
using Hrms.Domain.Entities;
using Hrms.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Hrms.Application.Services;

public class JobApplicationService(
    IHrmsRepository<JobApplication> applications,
    IHrmsRepository<Candidate> candidates,
    IHrmsRepository<Vacancy> vacancies) : IJobApplicationService
{
    public Task<List<JobApplication>> GetAllAsync(ApplicationFilter? filter = null, CancellationToken ct = default)
    {
        filter ??= new ApplicationFilter();
        var query = applications.Query()
            .Include(a => a.Candidate).ThenInclude(c => c.Resume)
            .Include(a => a.Vacancy)
            .Include(a => a.Interviews)
            .AsQueryable();

        if (filter.VacancyId is { } vacancyId)
            query = query.Where(a => a.VacancyId == vacancyId);

        if (filter.Stage is { } stage)
            query = query.Where(a => a.Stage == stage);

        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            var term = filter.Search.Trim();
            query = query.Where(a =>
                a.Candidate.LastName.Contains(term) ||
                a.Candidate.FirstName.Contains(term) ||
                a.Candidate.Email.Contains(term) ||
                a.Vacancy.Title.Contains(term));
        }

        return query.OrderByDescending(a => a.AppliedAt).ToListAsync(ct);
    }

    public Task<JobApplication?> GetByIdAsync(int id, CancellationToken ct = default) =>
        applications.Query()
            .Include(a => a.Candidate).ThenInclude(c => c.Resume)
            .Include(a => a.Vacancy)
            .Include(a => a.Interviews)
            .FirstOrDefaultAsync(a => a.Id == id, ct);

    public async Task AddAsync(JobApplication application, CancellationToken ct = default)
    {
        var candidate = await candidates.Query()
            .Include(c => c.Resume)
            .FirstOrDefaultAsync(c => c.Id == application.CandidateId, ct)
            ?? throw new InvalidOperationException("Кандидат не найден.");

        if (candidate.Resume is null)
            throw new InvalidOperationException("Сначала заполните резюме кандидата — без него отклик создать нельзя.");

        var vacancy = await vacancies.GetByIdAsync(application.VacancyId, ct)
            ?? throw new InvalidOperationException("Вакансия не найдена.");

        if (await applications.Query().AnyAsync(a => a.CandidateId == application.CandidateId && a.VacancyId == application.VacancyId, ct))
            throw new InvalidOperationException("У этого кандидата уже есть отклик на выбранную вакансию.");

        application.AppliedAt = application.AppliedAt == default ? DateTime.Now : application.AppliedAt;
        application.UpdatedAt = DateTime.Now;
        application.Source = Normalize(application.Source);
        application.Notes = Normalize(application.Notes);
        application.Candidate = candidate;
        application.Vacancy = vacancy;
        applications.Add(application);
        await applications.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(JobApplication application, CancellationToken ct = default)
    {
        var existing = await applications.GetByIdAsync(application.Id, ct)
            ?? throw new InvalidOperationException("Отклик не найден.");

        var candidate = await candidates.Query()
            .Include(c => c.Resume)
            .FirstOrDefaultAsync(c => c.Id == application.CandidateId, ct)
            ?? throw new InvalidOperationException("Кандидат не найден.");
        if (candidate.Resume is null)
            throw new InvalidOperationException("У кандидата нет резюме — отклик нельзя сохранить.");

        _ = await vacancies.GetByIdAsync(application.VacancyId, ct)
            ?? throw new InvalidOperationException("Вакансия не найдена.");

        if (existing.CandidateId != application.CandidateId || existing.VacancyId != application.VacancyId)
        {
            var duplicate = await applications.Query().AnyAsync(a =>
                a.Id != application.Id &&
                a.CandidateId == application.CandidateId &&
                a.VacancyId == application.VacancyId, ct);
            if (duplicate)
                throw new InvalidOperationException("У этого кандидата уже есть отклик на выбранную вакансию.");
        }

        existing.CandidateId = application.CandidateId;
        existing.VacancyId = application.VacancyId;
        existing.Stage = application.Stage;
        existing.AppliedAt = application.AppliedAt;
        existing.Source = Normalize(application.Source);
        existing.Notes = Normalize(application.Notes);
        existing.UpdatedAt = DateTime.Now;
        await applications.SaveChangesAsync(ct);
    }

    public async Task ChangeStageAsync(int id, ApplicationStage stage, CancellationToken ct = default)
    {
        var existing = await applications.GetByIdAsync(id, ct)
            ?? throw new InvalidOperationException("Отклик не найден.");
        existing.Stage = stage;
        existing.UpdatedAt = DateTime.Now;
        await applications.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        var existing = await applications.GetByIdAsync(id, ct)
            ?? throw new InvalidOperationException("Отклик не найден.");
        applications.Remove(existing);
        await applications.SaveChangesAsync(ct);
    }

    public Task<List<ApplicationOptionDto>> GetOptionsAsync(CancellationToken ct = default) =>
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

    private static string? Normalize(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}

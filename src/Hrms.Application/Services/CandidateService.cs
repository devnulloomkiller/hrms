using Hrms.Application.Interfaces;
using Hrms.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Hrms.Application.Services;

public class CandidateService(IHrmsRepository<Candidate> candidates) : ICandidateService
{
    public Task<List<Candidate>> GetAllAsync(CancellationToken ct = default) =>
        candidates.Query()
            .Include(c => c.Resumes)
            .OrderBy(c => c.LastName).ThenBy(c => c.FirstName)
            .ToListAsync(ct)!;

    public Task<Candidate?> GetByIdAsync(int id, CancellationToken ct = default) =>
        candidates.Query()
            .Include(c => c.Resumes).ThenInclude(r => r.Vacancy)
            .FirstOrDefaultAsync(c => c.Id == id, ct);

    public Task AddAsync(Candidate candidate, CancellationToken ct = default)
    {
        candidates.Add(candidate);
        return candidates.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(Candidate candidate, CancellationToken ct = default)
    {
        var existing = await candidates.GetByIdAsync(candidate.Id, ct)
            ?? throw new InvalidOperationException($"Candidate {candidate.Id} not found");
        existing.LastName = candidate.LastName;
        existing.FirstName = candidate.FirstName;
        existing.Email = candidate.Email;
        existing.Phone = candidate.Phone;
        await candidates.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        var existing = await candidates.GetByIdAsync(id, ct)
            ?? throw new InvalidOperationException($"Candidate {id} not found");
        candidates.Remove(existing);
        await candidates.SaveChangesAsync(ct);
    }
}

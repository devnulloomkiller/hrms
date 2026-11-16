using Hrms.Domain.Enums;

namespace Hrms.Domain.Entities;

public class Resume
{
    public int Id { get; set; }

    public int CandidateId { get; set; }

    public Candidate Candidate { get; set; } = null!;

    public int? VacancyId { get; set; }

    public Vacancy? Vacancy { get; set; }

    public required string Summary { get; set; }

    public int ExperienceYears { get; set; }

    public DateTime SubmittedAt { get; set; }

    public ResumeStatus Status { get; set; }

    public ResumeDetails? Details { get; set; }

    public List<Interview> Interviews { get; set; } = [];
}

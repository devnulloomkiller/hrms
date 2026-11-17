namespace Hrms.Domain.Entities;

public class Resume
{
    public int Id { get; set; }

    public int CandidateId { get; set; }

    public Candidate Candidate { get; set; } = null!;

    /// <summary>Желаемая должность — по ней резюме проще сопоставлять с вакансией.</summary>
    public required string Headline { get; set; }

    public required string Summary { get; set; }

    public int ExperienceYears { get; set; }

    public decimal? DesiredSalary { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public ResumeDetails? Details { get; set; }

    public List<ResumeVacancyMatch> VacancyMatches { get; set; } = [];
}

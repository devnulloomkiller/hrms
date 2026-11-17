using System.ComponentModel.DataAnnotations;
using Hrms.Domain.Enums;

namespace Hrms.Domain.Entities;

/// <summary>Отклик кандидата на вакансию и его текущий этап воронки.</summary>
public class JobApplication
{
    public int Id { get; set; }

    public int CandidateId { get; set; }

    public Candidate Candidate { get; set; } = null!;

    public int VacancyId { get; set; }

    public Vacancy Vacancy { get; set; } = null!;

    public ApplicationStage Stage { get; set; }

    public DateTime AppliedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    [MaxLength(100)]
    public string? Source { get; set; }

    [MaxLength(1000)]
    public string? Notes { get; set; }

    public List<Interview> Interviews { get; set; } = [];
}

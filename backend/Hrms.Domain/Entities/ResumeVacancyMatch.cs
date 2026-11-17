using Hrms.Domain.Enums;

namespace Hrms.Domain.Entities;

/// <summary>Подбор резюме к вакансии: связь N:N с оценкой соответствия и статусом.</summary>
public class ResumeVacancyMatch
{
    public int ResumeId { get; set; }

    public Resume Resume { get; set; } = null!;

    public int VacancyId { get; set; }

    public Vacancy Vacancy { get; set; } = null!;

    public MatchStatus Status { get; set; }

    public int? MatchScore { get; set; }

    public DateTime MatchedAt { get; set; }

    public string? Comment { get; set; }
}

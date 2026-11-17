using System.ComponentModel.DataAnnotations;
using Hrms.Domain.Enums;

namespace Hrms.Domain.Entities;

public class Vacancy
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Укажите название вакансии")]
    [MaxLength(200)]
    public required string Title { get; set; }

    [Required(ErrorMessage = "Укажите отдел")]
    [MaxLength(100)]
    public required string Department { get; set; }

    [MaxLength(4000)]
    public string? Description { get; set; }

    [Range(0, 10_000_000, ErrorMessage = "Зарплата должна быть положительной")]
    public decimal SalaryMin { get; set; }

    [Range(0, 10_000_000, ErrorMessage = "Зарплата должна быть положительной")]
    public decimal? SalaryMax { get; set; }

    /// <summary>Минимальный опыт в годах — используется в подборе резюме.</summary>
    [Range(0, 60, ErrorMessage = "Опыт должен быть от 0 до 60 лет")]
    public int MinExperience { get; set; }

    /// <summary>Ключевые навыки через запятую — участвуют в оценке соответствия.</summary>
    [MaxLength(1000)]
    public string? RequiredSkills { get; set; }

    public VacancyStatus Status { get; set; }

    public DateOnly PublishedAt { get; set; }

    public DateOnly? AcceptingResumesUntil { get; set; }

    public List<JobApplication> Applications { get; set; } = [];

    public List<ResumeVacancyMatch> ResumeMatches { get; set; } = [];
}

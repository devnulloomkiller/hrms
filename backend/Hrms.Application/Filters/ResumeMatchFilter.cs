namespace Hrms.Application.Filters;

/// <summary>Фильтры подбора резюме под конкретную вакансию.</summary>
public record ResumeMatchFilter(
    int? MinExperience = null,
    string? Skill = null,
    decimal? MaxDesiredSalary = null,
    bool OnlyOpenVacancy = true,
    bool OnlyNotLinked = true);

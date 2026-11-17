namespace Hrms.Application.Filters;

public record ResumeFilter(
    string? Search = null,
    int? MinExperience = null,
    string? Skill = null,
    decimal? MaxDesiredSalary = null);

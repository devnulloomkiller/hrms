namespace Hrms.Application.Filters;

/// <summary>Фильтры подбора вакансий под конкретное резюме.</summary>
public record VacancyMatchFilter(
    string? Search = null,
    bool OnlyOpen = true,
    bool OnlyAccepting = true,
    bool OnlyNotLinked = true);

using Hrms.Domain.Enums;

namespace Hrms.Application.Filters;

public record VacancyFilter(
    string? Search = null,
    VacancyStatus? Status = null,
    bool OnlyAccepting = false);

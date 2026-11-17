using Hrms.Domain.Enums;

namespace Hrms.Application.Filters;

public record ApplicationFilter(
    string? Search = null,
    int? VacancyId = null,
    ApplicationStage? Stage = null);

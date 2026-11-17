using Hrms.Domain.Enums;

namespace Hrms.Application.Filters;

public record InterviewFilter(
    bool? Upcoming = null,
    InterviewResult? Result = null,
    int? VacancyId = null);

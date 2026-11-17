using Hrms.Application.DTOs;
using Hrms.Domain.Entities;
using Hrms.Domain.Enums;

namespace Hrms.Web.ViewModels;

public sealed class ApplicationBoardViewModel
{
    public IReadOnlyList<ApplicationColumn> Columns { get; init; } = [];

    public IReadOnlyList<JobApplication> ClosedApplications { get; init; } = [];

    public IReadOnlyList<VacancyOptionDto> Vacancies { get; init; } = [];

    public string? Search { get; init; }

    public int? VacancyId { get; init; }

    public ApplicationStage? Stage { get; init; }

    public int TotalCount { get; init; }
}

public sealed record ApplicationColumn(
    ApplicationStage Stage,
    string Title,
    IReadOnlyList<JobApplication> Items);

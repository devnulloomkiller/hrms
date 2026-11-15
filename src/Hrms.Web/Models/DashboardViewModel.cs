using Hrms.Domain.Entities;

namespace Hrms.Web.Models;

public sealed class DashboardViewModel
{
    public int OpenVacancies { get; init; }

    public int TotalCandidates { get; init; }

    public int ActiveResumes { get; init; }

    public int UpcomingInterviewsCount { get; init; }

    public IReadOnlyList<Interview> UpcomingInterviews { get; init; } = [];

    public IReadOnlyList<Vacancy> ExpiringVacancies { get; init; } = [];

    public IReadOnlyList<Resume> RecentResumes { get; init; } = [];
}

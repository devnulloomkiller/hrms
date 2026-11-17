using Hrms.Domain.Entities;
using Hrms.Domain.Enums;

namespace Hrms.Web.ViewModels;

public sealed class DashboardViewModel
{
    public int OpenVacancies { get; init; }

    public int TotalCandidates { get; init; }

    public int ActiveApplications { get; init; }

    public int UpcomingInterviewsCount { get; init; }

    public int HiredCount { get; init; }

    public IReadOnlyList<Interview> UpcomingInterviews { get; init; } = [];

    public IReadOnlyList<Vacancy> ExpiringVacancies { get; init; } = [];

    public IReadOnlyList<JobApplication> RecentApplications { get; init; } = [];

    public IReadOnlyDictionary<ApplicationStage, int> Pipeline { get; init; } =
        new Dictionary<ApplicationStage, int>();
}

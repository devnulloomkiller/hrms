using Hrms.Domain.Enums;

namespace Hrms.Application.Filters;

public record CandidateFilter(
    string? Search = null,
    int? MinExperience = null,
    string? Skill = null,
    bool OnlyWithResume = false);

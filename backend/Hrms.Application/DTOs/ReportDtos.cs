using Hrms.Domain.Enums;

namespace Hrms.Application.DTOs;

public record ExpiringVacancyDto(
    int VacancyId,
    string Title,
    string Department,
    DateOnly AcceptingResumesUntil,
    int DaysLeft,
    int ApplicationCount,
    int MatchedResumeCount);

public record CandidateApplicationDto(
    int ApplicationId,
    int VacancyId,
    string VacancyTitle,
    string Department,
    ApplicationStage Stage,
    DateTime AppliedAt,
    int InterviewCount);

public record CandidateVacancyMatchDto(
    int VacancyId,
    string VacancyTitle,
    string Department,
    VacancyStatus Status,
    MatchStatus MatchStatus,
    int? MatchScore);

public record CandidateProfileDto(
    int CandidateId,
    string FullName,
    string Email,
    string? Phone,
    string? Headline,
    int ExperienceYears,
    decimal? DesiredSalary,
    string? Skills,
    string? Education,
    string? Summary,
    int TotalApplications,
    int TotalInterviews,
    List<CandidateApplicationDto> Applications,
    List<CandidateVacancyMatchDto> Matches);

public record VacancyInterviewSummaryDto(
    int VacancyId,
    string Title,
    string Department,
    int TotalInterviews,
    int Passed,
    int Failed,
    int Scheduled,
    int Cancelled);

public record VacancyPipelineDto(
    int VacancyId,
    string Title,
    string Department,
    VacancyStatus Status,
    int TotalApplications,
    int Applied,
    int Screening,
    int Interview,
    int Offer,
    int Hired,
    int Rejected,
    int Withdrawn);

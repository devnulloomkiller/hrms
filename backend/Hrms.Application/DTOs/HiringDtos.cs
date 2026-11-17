using Hrms.Domain.Enums;

namespace Hrms.Application.DTOs;

/// <summary>Строка подбора: резюме и насколько оно подходит вакансии.</summary>
public record ResumeMatchDto(
    int ResumeId,
    int CandidateId,
    string CandidateName,
    string Headline,
    int ExperienceYears,
    decimal? DesiredSalary,
    string? Skills,
    int MatchScore,
    bool IsLinked,
    MatchStatus? LinkStatus,
    ApplicationStage? ApplicationStage);

/// <summary>Строка подбора: вакансия и насколько она подходит резюме.</summary>
public record VacancyMatchDto(
    int VacancyId,
    string Title,
    string Department,
    VacancyStatus Status,
    DateOnly? AcceptingResumesUntil,
    decimal SalaryMin,
    decimal? SalaryMax,
    int MinExperience,
    string? RequiredSkills,
    int MatchScore,
    bool IsLinked,
    MatchStatus? LinkStatus,
    ApplicationStage? ApplicationStage);

public record CandidateOptionDto(
    int Id,
    string FullName,
    string Email,
    string Headline,
    int ExperienceYears,
    bool HasResume);

public record VacancyOptionDto(
    int Id,
    string Title,
    string Department,
    VacancyStatus Status);

public record ApplicationOptionDto(
    int Id,
    string CandidateName,
    string VacancyTitle,
    ApplicationStage Stage);

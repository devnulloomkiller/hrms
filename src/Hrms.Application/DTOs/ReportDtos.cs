using Hrms.Domain.Entities;

namespace Hrms.Application.DTOs;

public record ExpiringVacancyDto(
    int VacancyId,
    string Title,
    string Department,
    DateOnly AcceptingResumesUntil,
    int DaysLeft,
    int ResumeCount);

public record CandidateResumeDto(
    int ResumeId,
    string? VacancyTitle,
    ResumeStatus Status,
    int ExperienceYears,
    DateOnly SubmittedAt,
    List<InterviewInfoDto> Interviews);

public record InterviewInfoDto(
    DateTime ScheduledAt,
    InterviewMode Mode,
    InterviewResult Result,
    string VacancyTitle,
    List<string> Participants);

public record CandidateProfileDto(
    int CandidateId,
    string FullName,
    string Email,
    string? Phone,
    int TotalResumes,
    int TotalInterviews,
    List<CandidateResumeDto> Resumes);

public record VacancyInterviewSummaryDto(
    int VacancyId,
    string Title,
    string Department,
    int TotalInterviews,
    int Passed,
    int Failed,
    int Scheduled,
    int Cancelled);

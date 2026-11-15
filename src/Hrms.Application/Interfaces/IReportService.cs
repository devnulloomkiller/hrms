using Hrms.Application.DTOs;

namespace Hrms.Application.Interfaces;

public interface IReportService
{
    /// <summary>Вакансии со сроком приёма резюме, истекающим в ближайшие дни, со счётчиком поданных резюме.</summary>
    Task<List<ExpiringVacancyDto>> GetVacanciesWithExpiringDeadlineAsync(int days, CancellationToken ct = default);

    /// <summary>Полная карточка кандидата: все резюме, детали и собеседования с участниками.</summary>
    Task<CandidateProfileDto?> GetCandidateProfileAsync(int candidateId, CancellationToken ct = default);

    /// <summary>Сводка по вакансиям: количество собеседований с группировкой по результату, за период.</summary>
    Task<List<VacancyInterviewSummaryDto>> GetInterviewSummaryByVacancyAsync(DateTime from, DateTime to, CancellationToken ct = default);
}

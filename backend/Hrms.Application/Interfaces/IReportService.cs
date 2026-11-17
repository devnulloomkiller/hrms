using Hrms.Application.DTOs;

namespace Hrms.Application.Interfaces;

public interface IReportService
{
    /// <summary>Вакансии со сроком приёма резюме, истекающим в ближайшие дни, со счётчиками откликов и подборов.</summary>
    Task<List<ExpiringVacancyDto>> GetVacanciesWithExpiringDeadlineAsync(int days, CancellationToken ct = default);

    /// <summary>Карточка кандидата: резюме 1:1, отклики, собеседования и подборы.</summary>
    Task<CandidateProfileDto?> GetCandidateProfileAsync(int candidateId, CancellationToken ct = default);

    /// <summary>Сводка по вакансиям: количество собеседований с группировкой по результату, за период.</summary>
    Task<List<VacancyInterviewSummaryDto>> GetInterviewSummaryByVacancyAsync(DateTime from, DateTime to, CancellationToken ct = default);

    /// <summary>Воронка по вакансиям: отклики по этапам и конверсия в найм.</summary>
    Task<List<VacancyPipelineDto>> GetPipelineByVacancyAsync(CancellationToken ct = default);
}

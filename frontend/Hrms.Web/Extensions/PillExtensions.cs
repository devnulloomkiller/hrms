using Hrms.Domain.Enums;

namespace Hrms.Web.Extensions;

public static class PillExtensions
{
    public static string Pill(this VacancyStatus s) => Wrap(s.Ru(), s.ToString().ToLowerInvariant());
    public static string Pill(this ApplicationStage s) => Wrap(s.Ru(), s.ToString().ToLowerInvariant());
    public static string Pill(this MatchStatus s) => Wrap(s.Ru(), s.ToString().ToLowerInvariant());
    public static string Pill(this InterviewResult r) => Wrap(r.Ru(), r.ToString().ToLowerInvariant());
    public static string Pill(this InterviewMode m) => Wrap(m.Ru(), m.ToString().ToLowerInvariant());

    public static string Ru(this VacancyStatus s) => s switch
    {
        VacancyStatus.Draft => "Черновик",
        VacancyStatus.Open => "Открыта",
        VacancyStatus.Paused => "Пауза",
        VacancyStatus.Closed => "Закрыта",
        _ => s.ToString(),
    };

    public static string Ru(this ApplicationStage s) => s switch
    {
        ApplicationStage.Applied => "Новый отклик",
        ApplicationStage.Screening => "Скрининг",
        ApplicationStage.Interview => "Собеседование",
        ApplicationStage.Offer => "Оффер",
        ApplicationStage.Hired => "Нанят",
        ApplicationStage.Rejected => "Отказ",
        ApplicationStage.Withdrawn => "Отозван",
        _ => s.ToString(),
    };

    public static string Ru(this MatchStatus s) => s switch
    {
        MatchStatus.Suggested => "Предложен",
        MatchStatus.Shortlisted => "Шортлист",
        MatchStatus.Rejected => "Не подходит",
        _ => s.ToString(),
    };

    public static string Ru(this InterviewResult r) => r switch
    {
        InterviewResult.Scheduled => "Запланировано",
        InterviewResult.Passed => "Пройдено",
        InterviewResult.Failed => "Провалено",
        InterviewResult.Cancelled => "Отменено",
        _ => r.ToString(),
    };

    public static string Ru(this InterviewMode m) => m switch
    {
        InterviewMode.Onsite => "Очно",
        InterviewMode.Phone => "Телефон",
        InterviewMode.Video => "Видео",
        _ => m.ToString(),
    };

    private static string Wrap(string label, string css) =>
        $"<span class=\"pill pill-{css}\">{label}</span>";
}

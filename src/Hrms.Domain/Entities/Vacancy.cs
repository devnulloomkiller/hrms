namespace Hrms.Domain.Entities;

public class Vacancy
{
    public int Id { get; set; }

    public required string Title { get; set; }

    public required string Department { get; set; }

    public string? Description { get; set; }

    public decimal SalaryMin { get; set; }

    public decimal? SalaryMax { get; set; }

    public VacancyStatus Status { get; set; }

    public DateOnly PublishedAt { get; set; }

    public DateOnly? AcceptingResumesUntil { get; set; }

    public List<Interview> Interviews { get; set; } = [];
}

public enum VacancyStatus
{
    Draft = 0,
    Open = 1,
    Paused = 2,
    Closed = 3,
}

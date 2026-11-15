namespace Hrms.Domain.Entities;

public class Interview
{
    public int Id { get; set; }

    public int ResumeId { get; set; }

    public Resume Resume { get; set; } = null!;

    public int VacancyId { get; set; }

    public Vacancy Vacancy { get; set; } = null!;

    public DateTime ScheduledAt { get; set; }

    public InterviewMode Mode { get; set; }

    public InterviewResult Result { get; set; }

    public string? Notes { get; set; }

    public List<InterviewInterviewer> Participants { get; set; } = [];
}

public enum InterviewMode
{
    Onsite = 0,
    Phone = 1,
    Video = 2,
}

public enum InterviewResult
{
    Scheduled = 0,
    Passed = 1,
    Failed = 2,
    Cancelled = 3,
}

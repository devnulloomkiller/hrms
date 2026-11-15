namespace Hrms.Domain.Entities;

public class Interviewer
{
    public int Id { get; set; }

    public required string FullName { get; set; }

    public string? Email { get; set; }

    public string? Position { get; set; }

    public List<InterviewInterviewer> Interviews { get; set; } = [];
}

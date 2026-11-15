namespace Hrms.Domain.Entities;

public class InterviewInterviewer
{
    public int InterviewId { get; set; }

    public Interview Interview { get; set; } = null!;

    public int InterviewerId { get; set; }

    public Interviewer Interviewer { get; set; } = null!;

    public string Role { get; set; } = "Interviewer";
}

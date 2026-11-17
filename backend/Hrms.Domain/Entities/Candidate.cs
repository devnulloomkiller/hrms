namespace Hrms.Domain.Entities;

public class Candidate
{
    public int Id { get; set; }

    public required string LastName { get; set; }

    public required string FirstName { get; set; }

    public required string Email { get; set; }

    public string? Phone { get; set; }

    public Resume? Resume { get; set; }

    public List<JobApplication> Applications { get; set; } = [];
}

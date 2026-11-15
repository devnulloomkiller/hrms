namespace Hrms.Domain.Entities;

public class ResumeDetails
{
    public int ResumeId { get; set; }

    public Resume Resume { get; set; } = null!;

    public string? Education { get; set; }

    public string? Skills { get; set; }

    public string? FilePath { get; set; }
}

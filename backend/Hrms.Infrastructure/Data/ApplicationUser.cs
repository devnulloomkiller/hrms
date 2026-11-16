using Microsoft.AspNetCore.Identity;

namespace Hrms.Infrastructure.Data;

public class ApplicationUser : IdentityUser
{
    public string? DisplayName { get; set; }
}

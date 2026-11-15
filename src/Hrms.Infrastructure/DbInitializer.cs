using Hrms.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;

namespace Hrms.Infrastructure;

public static class DbInitializer
{
    public const string AdminRole = "Admin";
    public const string HrRole = "HR";

    public static async Task SeedRolesAndAdminAsync(
        RoleManager<IdentityRole> roles, UserManager<ApplicationUser> users)
    {
        foreach (var role in new[] { AdminRole, HrRole })
            if (!await roles.RoleExistsAsync(role))
                await roles.CreateAsync(new IdentityRole(role));

        const string adminEmail = "admin@hrms.local";
        const string adminPassword = "Admin123";
        var admin = await users.FindByEmailAsync(adminEmail);
        if (admin is null)
        {
            admin = new ApplicationUser { UserName = adminEmail, Email = adminEmail, EmailConfirmed = true, DisplayName = "Administrator" };
            var result = await users.CreateAsync(admin, adminPassword);
            if (result.Succeeded)
                await users.AddToRoleAsync(admin, AdminRole);
        }
    }
}

using ECommerce.Domain.Constants;
using ECommerce.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace ECommerce.Infrastructure.Persistence.Seeding;

public sealed class IdentitySeeder(
    RoleManager<ApplicationRole> roleManager,
    UserManager<ApplicationUser> userManager,
    IConfiguration config) : IDataSeeder
{
    public int Order => 0;

    public async Task SeedAsync(CancellationToken ct = default)
    {
        await SeedRolesAsync(ct);
        await SeedSuperAdminAsync(ct);
    }

    private async Task SeedRolesAsync(CancellationToken ct)
    {
        if (await roleManager.Roles.AnyAsync(ct))
            return;

        foreach (var roleName in Roles.All)
        {
            await roleManager.CreateAsync(new ApplicationRole(roleName)
            {
                Description = $"{roleName} system role"
            });
        }
    }

    private async Task SeedSuperAdminAsync(CancellationToken ct)
    {
        var section = config.GetSection("Seed:SuperAdmin");
        var email = section["Email"];
        var password = section["Password"];

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            return;

        if (await userManager.Users.AnyAsync(u => u.Email == email, ct))
            return;

        var user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            EmailConfirmed = true,
            DisplayName = section["DisplayName"] ?? "Super Admin"
        };

        var result = await userManager.CreateAsync(user, password);
        if (result.Succeeded)
            await userManager.AddToRoleAsync(user, Roles.SuperAdmin);
    }
}

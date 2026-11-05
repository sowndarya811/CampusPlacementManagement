
using CampusPlacementManagement.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CampusPlacementManagement.Data;

public static class IdentitySeeder
{
    public static async Task SeedRolesAsync(
        IServiceProvider serviceProvider)
    {
        var roleManager = serviceProvider
            .GetRequiredService<RoleManager<IdentityRole>>();

        var userManager = serviceProvider
            .GetRequiredService<UserManager<ApplicationUser>>();

        var configuration = serviceProvider
            .GetRequiredService<IConfiguration>();

        // 1. Create default roles
        string[] roles =
        {
            "SuperAdmin",
            "PlacementOfficer",
            "CompanyHR",
            "Student"
        };

        foreach (var roleName in roles)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                var roleResult = await roleManager.CreateAsync(
                    new IdentityRole(roleName));

                if (!roleResult.Succeeded)
                {
                    throw new InvalidOperationException(
                        $"Failed to create role {roleName}: " +
                        string.Join(", ",
                            roleResult.Errors.Select(e => e.Description)));
                }
            }
        }

        // 2. Read initial SuperAdmin configuration
        var adminEmail =
            configuration["BootstrapAdmin:Email"];

        var adminPassword =
            configuration["BootstrapAdmin:Password"];

        var adminFullName =
            configuration["BootstrapAdmin:FullName"];

        // Bootstrap account is optional until configured
        if (string.IsNullOrWhiteSpace(adminEmail) &&
            string.IsNullOrWhiteSpace(adminPassword) &&
            string.IsNullOrWhiteSpace(adminFullName))
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(adminEmail) ||
            string.IsNullOrWhiteSpace(adminPassword) ||
            string.IsNullOrWhiteSpace(adminFullName))
        {
            throw new InvalidOperationException(
                "All BootstrapAdmin settings must be provided.");
        }

        // 3. Check whether the admin account already exists
        var existingAdmin =
            await userManager.FindByEmailAsync(adminEmail);

        if (existingAdmin != null)
        {
            var isSuperAdmin =
                await userManager.IsInRoleAsync(
                    existingAdmin, "SuperAdmin");

            if (!isSuperAdmin)
            {
                throw new InvalidOperationException(
                    "Bootstrap admin email already belongs to " +
                    "a user without the SuperAdmin role. " +
                    "Use a separate admin email.");
            }

            return;
        }

        // 4. Create initial SuperAdmin account
        var adminUser = new ApplicationUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            FullName = adminFullName,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            EmailConfirmed = true
        };

        var createResult = await userManager.CreateAsync(
            adminUser, adminPassword);

        if (!createResult.Succeeded)
        {
            throw new InvalidOperationException(
                "Failed to create SuperAdmin: " +
                string.Join(", ",
                    createResult.Errors.Select(e => e.Description)));
        }

        // 5. Assign SuperAdmin role
        var roleAssignResult =
            await userManager.AddToRoleAsync(
                adminUser, "SuperAdmin");

        if (!roleAssignResult.Succeeded)
        {
            throw new InvalidOperationException(
                "Failed to assign SuperAdmin role: " +
                string.Join(", ",
                    roleAssignResult.Errors.Select(e => e.Description)));
        }
    }
}

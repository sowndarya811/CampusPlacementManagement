
using CampusPlacementManagement.Interfaces;
using CampusPlacementManagement.Models;
using Microsoft.AspNetCore.Identity;

namespace CampusPlacementManagement.Services;

public class RoleService : IRoleService
{
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly UserManager<ApplicationUser> _userManager;

    public RoleService(
        RoleManager<IdentityRole> roleManager,
        UserManager<ApplicationUser> userManager)
    {
        _roleManager = roleManager;
        _userManager = userManager;
    }

    // 1. Create a new role
    public async Task CreateRoleAsync(string roleName)
    {
        if (string.IsNullOrWhiteSpace(roleName))
        {
            throw new ArgumentException(
                "Role name is required.");
        }

        roleName = roleName.Trim();

        var existingRole =
            await _roleManager.RoleExistsAsync(roleName);

        if (existingRole)
        {
            throw new InvalidOperationException(
                "Role already exists.");
        }

        var role = new IdentityRole(roleName);

        var result =
            await _roleManager.CreateAsync(role);

        if (!result.Succeeded)
        {
            throw new InvalidOperationException(
                string.Join(", ",
                    result.Errors.Select(e => e.Description)));
        }
    }

    // 2. Get all available roles
    public async Task<IList<string>> GetAllRolesAsync()
    {
        return await Task.FromResult(
            _roleManager.Roles
                .OrderBy(role => role.Name)
                .Select(role => role.Name!)
                .ToList());
    }

    // 3. Assign a role to a user
    public async Task AssignRoleAsync(
        string userId,
        string roleName)
    {
        if (string.IsNullOrWhiteSpace(userId))
        {
            throw new ArgumentException(
                "User ID is required.");
        }

        if (string.IsNullOrWhiteSpace(roleName))
        {
            throw new ArgumentException(
                "Role name is required.");
        }

        userId = userId.Trim();
        roleName = roleName.Trim();

        // Find user
        var user =
            await _userManager.FindByIdAsync(userId);

        if (user == null)
        {
            throw new KeyNotFoundException(
                "User not found.");
        }

        // Check whether role exists
        var roleExists =
            await _roleManager.RoleExistsAsync(roleName);

        if (!roleExists)
        {
            throw new KeyNotFoundException(
                "Role not found.");
        }

        // Get user's existing roles
        var currentRoles =
            await _userManager.GetRolesAsync(user);

        // Prevent duplicate role assignment
        if (currentRoles.Contains(
            roleName,
            StringComparer.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(
                "User already has this role.");
        }

        // Assign role
        var result =
            await _userManager.AddToRoleAsync(
                user,
                roleName);

        if (!result.Succeeded)
        {
            throw new InvalidOperationException(
                string.Join(", ",
                    result.Errors.Select(e => e.Description)));
        }
    }
}

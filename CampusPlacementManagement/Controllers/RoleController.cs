
using CampusPlacementManagement.DTOs.Auth;
using CampusPlacementManagement.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CampusPlacementManagement.Controllers;

[ApiController]
[Route("api/roles")]
[Authorize(Roles = "SuperAdmin")]
public class RoleController : ControllerBase
{
    private readonly IRoleService _roleService;

    public RoleController(IRoleService roleService)
    {
        _roleService = roleService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateRole(
        CreateRoleDto request)
    {
        await _roleService.CreateRoleAsync(request.RoleName);

        return Ok(new
        {
            message = "Role created successfully."
        });
    }

    [HttpGet]
    public async Task<IActionResult> GetAllRoles()
    {
        var roles = await _roleService.GetAllRolesAsync();

        return Ok(roles);
    }

    [HttpPost("assign")]
    public async Task<IActionResult> AssignRole(
        AssignRoleDto request)
    {
        await _roleService.AssignRoleAsync(
            request.UserId,
            request.RoleName);

        return Ok(new
        {
            message = "Role assigned successfully."
        });
    }
}

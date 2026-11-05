using System.Security.Claims;
using CampusPlacementManagement.DTOs.Placement;
using CampusPlacementManagement.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CampusPlacementManagement.Controllers;

[ApiController]
[Route("api/placements")]
[Authorize(Roles = "PlacementOfficer")]
public class PlacementsController : ControllerBase
{
    private readonly IPlacementService _service;

    public PlacementsController(IPlacementService service)
    {
        _service = service;
    }

    private string CurrentUserId =>
        User.FindFirstValue(ClaimTypes.NameIdentifier)
        ?? throw new UnauthorizedAccessException("User ID not found.");

    [HttpPost]
    public async Task<IActionResult> Create(PlacementCreateDto dto)
    {
        try
        {
            var result = await _service.CreateAsync(dto, CurrentUserId);
            return CreatedAtAction(nameof(GetById),
                new { id = result.PlacementId }, result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            return Ok(await _service.GetAllAsync(CurrentUserId));
        }
        catch (InvalidOperationException ex)
        {
            return Forbid();
        }
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            return Ok(await _service.GetByIdAsync(id, CurrentUserId));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException)
        {
            return Forbid();
        }
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(
        int id, PlacementUpdateDto dto)
    {
        try
        {
            return Ok(await _service.UpdateAsync(id, dto, CurrentUserId));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpPut("{id:int}/close")]
    public async Task<IActionResult> Close(int id)
    {
        try
        {
            await _service.CloseAsync(id, CurrentUserId);
            return Ok(new { message = "Placement closed successfully." });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }
}
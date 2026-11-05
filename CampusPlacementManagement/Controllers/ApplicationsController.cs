
using System.Security.Claims;
using CampusPlacementManagement.DTOs.Application;
using CampusPlacementManagement.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CampusPlacementManagement.Controllers;

[ApiController]
[Route("api/applications")]
public class ApplicationsController : ControllerBase
{
    private readonly IApplicationService _service;

    public ApplicationsController(IApplicationService service)
    {
        _service = service;
    }

    // Get currently authenticated user's ID
    private string CurrentUserId =>
        User.FindFirstValue(ClaimTypes.NameIdentifier)
        ?? throw new UnauthorizedAccessException("User ID not found.");

    // 1. Student applies for a placement
    [HttpPost]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> Apply(
        [FromBody] ApplicationCreateDto dto)
    {
        try
        {
            var result = await _service.ApplyAsync(dto, CurrentUserId);

            return CreatedAtAction(
                nameof(GetMyApplications),
                new { },
                result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new
            {
                message = ex.Message
            });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new
            {
                message = ex.Message
            });
        }
    }

    // 2. Student views their own applications
    [HttpGet("my")]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> GetMyApplications()
    {
        try
        {
            var result = await _service.GetMyApplicationsAsync(CurrentUserId);

            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new
            {
                message = ex.Message
            });
        }
    }

    // 3. Student withdraws an application
    [HttpPut("{id:int}/withdraw")]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> Withdraw(int id)
    {
        try
        {
            await _service.WithdrawAsync(id, CurrentUserId);

            return Ok(new
            {
                message = "Application withdrawn successfully."
            });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new
            {
                message = ex.Message
            });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new
            {
                message = ex.Message
            });
        }
    }

    // 4. Placement Officer views applicants for a placement
    [HttpGet("placement/{placementId:int}")]
    [Authorize(Roles = "PlacementOfficer")]
    public async Task<IActionResult> GetApplicants(int placementId)
    {
        try
        {
            var result = await _service.GetApplicantsAsync(
                placementId, CurrentUserId);

            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new
            {
                message = ex.Message
            });
        }
        catch (InvalidOperationException)
        {
            return Forbid();
        }
    }

    // 5. Placement Officer shortlists or rejects an application
    [HttpPut("{id:int}/status")]
    [Authorize(Roles = "PlacementOfficer")]
    public async Task<IActionResult> UpdateStatus(
        int id,
        [FromBody] ApplicationStatusUpdateDto dto)
    {
        try
        {
            var result = await _service.UpdateStatusAsync(
                id, dto, CurrentUserId);

            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new
            {
                message = ex.Message
            });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new
            {
                message = ex.Message
            });
        }
    }
}

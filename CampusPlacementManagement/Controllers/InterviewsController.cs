using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CampusPlacementManagement.Interfaces;
using System.Security.Claims;

namespace CampusPlacementManagement.Controllers;

[ApiController]
[Route("api/interviews")]
[Authorize]
public class InterviewsController : ControllerBase
{
    private readonly IInterviewService _service;

    public InterviewsController(IInterviewService service)
    {
        _service = service;
    }

    private string CurrentUserId =>
        User.FindFirstValue(ClaimTypes.NameIdentifier)
        ?? throw new UnauthorizedAccessException("User ID not found.");

    [HttpPut("{id:int}/complete")]
    [Authorize(Roles = "PlacementOfficer")]
    public async Task<IActionResult> Complete(int id)
    {
        try
        {
            await _service.CompleteAsync(id, CurrentUserId);

            return Ok(new
            {
                message = "Interview marked as completed successfully."
            });
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
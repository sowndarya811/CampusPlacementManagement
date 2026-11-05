
using CampusPlacementManagement.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CampusPlacementManagement.Controllers;

[ApiController]
[Route("api/dashboard")]
[Authorize(Roles = "SuperAdmin")]
public class DashboardController : ControllerBase
{
    private readonly IDashboardService _service;

    public DashboardController(IDashboardService service)
    {
        _service = service;
    }

    // 1. Overall Dashboard Summary
    [HttpGet("summary")]
    public async Task<IActionResult> GetSummary()
    {
        var result = await _service.GetSummaryAsync();

        return Ok(result);
    }

    // 2. Application Status Report
    [HttpGet("application-report")]
    public async Task<IActionResult> GetApplicationReport()
    {
        var result = await _service.GetApplicationReportAsync();

        return Ok(result);
    }

    // 3. Interview and Selection Report
    [HttpGet("interview-report")]
    public async Task<IActionResult> GetInterviewReport()
    {
        var result = await _service.GetInterviewReportAsync();

        return Ok(result);
    }

    // 4. Recent Placements
    [HttpGet("recent-placements")]
    public async Task<IActionResult> GetRecentPlacements()
    {
        var result = await _service.GetRecentPlacementsAsync();

        return Ok(result);
    }
}

using CampusPlacementManagement.DTOs.College;
using CampusPlacementManagement.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CampusPlacementManagement.Controllers;

[Route("api/colleges")]
[ApiController]
public class CollegesController : ControllerBase
{
    private readonly ICollegeService _collegeService;

    public CollegesController(ICollegeService collegeService)
    {
        _collegeService = collegeService;
    }

    // POST: api/colleges/register
    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register(
        [FromBody] CollegeCreateDto request)
    {
        var result = await _collegeService.RegisterAsync(request);

        return CreatedAtAction(
            nameof(GetById),
            new { id = result.CollegeId },
            result);
    }

    // GET: api/colleges
    [HttpGet]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _collegeService.GetAllAsync());
    }

    // GET: api/colleges/pending
    [HttpGet("pending")]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<IActionResult> GetPending()
    {
        return Ok(await _collegeService.GetPendingAsync());
    }

    // GET: api/colleges/{id}
    [HttpGet("{id:int}")]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<IActionResult> GetById(int id)
    {
        return Ok(await _collegeService.GetByIdAsync(id));
    }

    // PUT: api/colleges/{id}/approve
    [HttpPut("{id:int}/approve")]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<IActionResult> Approve(int id)
    {
        return Ok(await _collegeService.ApproveAsync(id));
    }

    // PUT: api/colleges/{id}/reject
    [HttpPut("{id:int}/reject")]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<IActionResult> Reject(int id)
    {
        return Ok(await _collegeService.RejectAsync(id));
    }

    // PUT: api/colleges/{id}/suspend
    [HttpPut("{id:int}/suspend")]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<IActionResult> Suspend(int id)
    {
        return Ok(await _collegeService.SuspendAsync(id));
    }

    // PUT: api/colleges/{id}
    [HttpPut("{id:int}")]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<IActionResult> Update(
        int id,
        [FromBody] CollegeUpdateDto request)
    {
        return Ok(await _collegeService.UpdateAsync(id, request));
    }
}
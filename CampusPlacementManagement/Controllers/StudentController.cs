
using System.Security.Claims;
using CampusPlacementManagement.DTOs.Student;
using CampusPlacementManagement.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CampusPlacementManagement.Controllers;

[Route("api/students")]
[ApiController]
[Authorize]
public class StudentController : ControllerBase
{
    private readonly IStudentService _studentService;

    public StudentController(IStudentService studentService)
    {
        _studentService = studentService;
    }

    // POST: api/students/enroll
    [HttpPost("enroll")]
    [Authorize(Roles = "PlacementOfficer")]
    public async Task<IActionResult> Enroll(
        [FromBody] StudentCreateDto request)
    {
        var result = await _studentService.EnrollAsync(request);

        return CreatedAtAction(
            nameof(GetById),
            new { id = result.StudentId },
            result);
    }

    // GET: api/students
    [HttpGet]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<IActionResult> GetAll()
    {
        var result = await _studentService.GetAllAsync();

        return Ok(result);
    }

    // GET: api/students/pending?collegeId=1
    [HttpGet("pending")]
    [Authorize(Roles = "SuperAdmin,PlacementOfficer")]
    public async Task<IActionResult> GetPending(
        [FromQuery] int collegeId)
    {
        if (collegeId <= 0)
            return BadRequest("Valid CollegeId is required.");

        var result =
            await _studentService.GetPendingAsync(collegeId);

        return Ok(result);
    }

    // GET: api/students/me
    [HttpGet("me")]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> GetMyProfile()
    {
        var userId =
            User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var result =
            await _studentService.GetByUserIdAsync(userId);

        return Ok(result);
    }

    // GET: api/students/{id}
    [HttpGet("{id:int}")]
    [Authorize(Roles = "SuperAdmin,PlacementOfficer,Student")]
    public async Task<IActionResult> GetById(int id)
    {
        var result =
            await _studentService.GetByIdAsync(id);

        // Student can access only their own profile
        if (User.IsInRole("Student"))
        {
            var userId =
                User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (result.UserId != userId)
                return Forbid();
        }

        return Ok(result);
    }

    // PUT: api/students/{id}/approve
    [HttpPut("{id:int}/approve")]
    [Authorize(Roles = "SuperAdmin,PlacementOfficer")]
    public async Task<IActionResult> Approve(int id)
    {
        var result =
            await _studentService.ApproveAsync(id);

        return Ok(result);
    }

    // PUT: api/students/{id}/reject
    [HttpPut("{id:int}/reject")]
    [Authorize(Roles = "SuperAdmin,PlacementOfficer")]
    public async Task<IActionResult> Reject(int id)
    {
        var result =
            await _studentService.RejectAsync(id);

        return Ok(result);
    }

    // PUT: api/students/{id}/suspend
    [HttpPut("{id:int}/suspend")]
    [Authorize(Roles = "SuperAdmin,PlacementOfficer")]
    public async Task<IActionResult> Suspend(int id)
    {
        var result =
            await _studentService.SuspendAsync(id);

        return Ok(result);
    }

    // PUT: api/students/{id}
    [HttpPut("{id:int}")]
    [Authorize(Roles = "SuperAdmin,PlacementOfficer,Student")]
    public async Task<IActionResult> Update(
        int id,
        [FromBody] StudentUpdateDto request)
    {
        // Student can update only their own profile
        if (User.IsInRole("Student"))
        {
            var userId =
                User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var ownProfile =
                await _studentService.GetByUserIdAsync(userId);

            if (ownProfile.StudentId != id)
                return Forbid();
        }

        var result =
            await _studentService.UpdateAsync(id, request);

        return Ok(result);
    }
}

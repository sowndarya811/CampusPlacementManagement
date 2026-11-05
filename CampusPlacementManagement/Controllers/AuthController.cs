
using CampusPlacementManagement.DTOs.Auth;
using CampusPlacementManagement.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CampusPlacementManagement.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    // POST: api/auth/register
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto request)
    {
        var response = await _authService.RegisterAsync(request);

        return Ok(new
        {
            message = "User registered successfully.",
            data = response
        });
    }

    // POST: api/auth/login
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto request)
    {
        var response = await _authService.LoginAsync(request);

        return Ok(new
        {
            message = "Login successful.",
            data = response
        });
    }
}


using CampusPlacementManagement.DTOs.Auth;

namespace CampusPlacementManagement.Interfaces;

public interface IAuthService
{
    Task<AuthResponseDto> RegisterAsync(RegisterDto request);

    Task<AuthResponseDto> LoginAsync(LoginDto request);
}

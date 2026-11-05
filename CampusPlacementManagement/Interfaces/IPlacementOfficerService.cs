using CampusPlacementManagement.DTOs.PlacementOfficer;

namespace CampusPlacementManagement.Interfaces;

public interface IPlacementOfficerService
{
    Task<PlacementOfficerResponseDto> RegisterAsync(
        PlacementOfficerRegisterDto dto);

    Task<List<PlacementOfficerResponseDto>> GetAllAsync();

    Task<List<PlacementOfficerResponseDto>> GetPendingAsync();

    Task<PlacementOfficerResponseDto?> GetByIdAsync(int id);

    Task<PlacementOfficerResponseDto?> GetMyProfileAsync(string userId);

    Task<bool> ApproveAsync(int id, string adminUserId);

    Task<bool> RejectAsync(int id);

    Task<bool> SuspendAsync(int id);
}
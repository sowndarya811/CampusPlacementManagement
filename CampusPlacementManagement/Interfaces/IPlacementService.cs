using CampusPlacementManagement.DTOs.Placement;

namespace CampusPlacementManagement.Interfaces;

public interface IPlacementService
{
    Task<PlacementResponseDto> CreateAsync(
        PlacementCreateDto dto, string userId);

    Task<List<PlacementResponseDto>> GetAllAsync(string userId);

    Task<PlacementResponseDto> GetByIdAsync(int id, string userId);

    Task<PlacementResponseDto> UpdateAsync(
        int id, PlacementUpdateDto dto, string userId);

    Task CloseAsync(int id, string userId);
}
using CampusPlacementManagement.DTOs.College;

namespace CampusPlacementManagement.Interfaces;

public interface ICollegeService
{
    Task<CollegeResponseDto> RegisterAsync(CollegeCreateDto request);

    Task<List<CollegeResponseDto>> GetAllAsync();

    Task<List<CollegeResponseDto>> GetPendingAsync();

    Task<CollegeResponseDto> GetByIdAsync(int id);

    Task<CollegeResponseDto> ApproveAsync(int id);

    Task<CollegeResponseDto> RejectAsync(int id);

    Task<CollegeResponseDto> SuspendAsync(int id);

    Task<CollegeResponseDto> UpdateAsync(
        int id,
        CollegeUpdateDto request);
}
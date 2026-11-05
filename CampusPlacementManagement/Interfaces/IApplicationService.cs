using CampusPlacementManagement.DTOs.Application;

namespace CampusPlacementManagement.Interfaces;

public interface IApplicationService
{
    Task<ApplicationResponseDto> ApplyAsync(
        ApplicationCreateDto dto, string userId);

    Task<List<ApplicationResponseDto>> GetMyApplicationsAsync(
        string userId);

    Task WithdrawAsync(int applicationId, string userId);

    Task<List<ApplicationResponseDto>> GetApplicantsAsync(
        int placementId, string userId);

    Task<ApplicationResponseDto> UpdateStatusAsync(
        int applicationId,
        ApplicationStatusUpdateDto dto,
        string userId);
}
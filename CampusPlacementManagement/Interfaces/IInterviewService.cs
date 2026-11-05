
using CampusPlacementManagement.DTOs.Interview;

namespace CampusPlacementManagement.Interfaces;

public interface IInterviewService
{
    Task<InterviewResponseDto> ScheduleAsync(
        InterviewCreateDto dto, string userId);

    Task<List<InterviewResponseDto>> GetMyInterviewsAsync(
        string userId);

    Task<List<InterviewResponseDto>> GetStudentInterviewsAsync(
        string userId);

    Task<InterviewResponseDto> UpdateAsync(
        int id, InterviewUpdateDto dto, string userId);

    Task CompleteAsync(
        int id, string userId);

    Task<InterviewResponseDto> UpdateSelectionAsync(
        int id, SelectionUpdateDto dto, string userId);
}

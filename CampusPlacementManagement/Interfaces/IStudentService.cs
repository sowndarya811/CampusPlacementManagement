using CampusPlacementManagement.DTOs.Student;

namespace CampusPlacementManagement.Interfaces;

public interface IStudentService
{
    Task<StudentResponseDto> EnrollAsync(StudentCreateDto request);

    Task<List<StudentResponseDto>> GetAllAsync();

    Task<List<StudentResponseDto>> GetPendingAsync(int collegeId);

    Task<StudentResponseDto> GetByIdAsync(int id);

    Task<StudentResponseDto> ApproveAsync(int id);

    Task<StudentResponseDto> RejectAsync(int id);

    Task<StudentResponseDto> SuspendAsync(int id);

    Task<StudentResponseDto> UpdateAsync(int id, StudentUpdateDto request);

    Task<StudentResponseDto> GetByUserIdAsync(string userId);
}
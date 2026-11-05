using CampusPlacementManagement.Models;

namespace CampusPlacementManagement.Interfaces;

public interface IInterviewRepository
{
    Task AddAsync(Interview interview);

    Task<Interview?> GetByIdAsync(int id);

    Task<List<Interview>> GetByPlacementOfficerAsync(int officerId);

    Task<List<Interview>> GetByStudentAsync(int studentId);

    Task<bool> HasScheduledInterviewAsync(int applicationId);

    Task SaveChangesAsync();
}
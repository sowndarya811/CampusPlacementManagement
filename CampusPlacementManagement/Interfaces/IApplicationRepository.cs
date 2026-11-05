using CampusPlacementManagement.Models;

namespace CampusPlacementManagement.Interfaces;

public interface IApplicationRepository
{
    Task AddAsync(PlacementApplication application);

    Task<PlacementApplication?> GetByIdAsync(int id);

    Task<bool> ExistsAsync(int studentId, int placementId);

    Task<List<PlacementApplication>> GetByStudentIdAsync(int studentId);

    Task<List<PlacementApplication>> GetByPlacementIdAsync(int placementId);

    Task SaveChangesAsync();
}
using CampusPlacementManagement.Models;

namespace CampusPlacementManagement.Interfaces;

public interface IPlacementRepository
{
    Task AddAsync(Placement placement);
    Task<Placement?> GetByIdAsync(int id);
    Task<List<Placement>> GetByCollegeIdAsync(int collegeId);
    Task SaveChangesAsync();
}
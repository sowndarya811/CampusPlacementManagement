using CampusPlacementManagement.Models;

namespace CampusPlacementManagement.Interfaces;

public interface ICollegeRepository
{
    Task<College> AddAsync(College college);

    Task<College?> GetByIdAsync(int id);

    Task<List<College>> GetAllAsync();

    Task<List<College>> GetPendingAsync();

    Task<bool> CodeExistsAsync(string code);

    Task<bool> EmailExistsAsync(string email);

    Task<bool> CodeExistsForOtherCollegeAsync(string code, int id);

    Task UpdateAsync(College college);

    Task<bool> SaveChangesAsync();
}
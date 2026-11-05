
using CampusPlacementManagement.Models;

namespace CampusPlacementManagement.Interfaces;

public interface ICompanyRepository
{
    Task<Company> AddAsync(Company company);

    Task<Company?> GetByIdAsync(int companyId);

    Task<List<Company>> GetAllAsync();

    Task<List<Company>> GetPendingAsync();

    Task UpdateAsync(Company company);

    Task<bool> ExistsByEmailAsync(string companyEmail);
}

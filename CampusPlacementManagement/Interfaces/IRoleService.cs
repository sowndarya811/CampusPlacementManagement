
namespace CampusPlacementManagement.Interfaces;

public interface IRoleService
{
    Task CreateRoleAsync(string roleName);

    Task<IList<string>> GetAllRolesAsync();

    Task AssignRoleAsync(string userId, string roleName);
}

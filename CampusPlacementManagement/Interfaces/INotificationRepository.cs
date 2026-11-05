
using CampusPlacementManagement.Models;

namespace CampusPlacementManagement.Interfaces;

public interface INotificationRepository
{
    Task AddAsync(Notification notification);

    Task<Notification?> GetByIdAsync(int id);

    Task<List<Notification>> GetByUserIdAsync(string userId);

    Task<List<Notification>> GetUnreadByUserIdAsync(string userId);

    Task<int> GetUnreadCountAsync(string userId);

    Task SaveChangesAsync();
}

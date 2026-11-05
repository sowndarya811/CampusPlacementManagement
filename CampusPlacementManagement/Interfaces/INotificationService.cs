
using CampusPlacementManagement.DTOs.Notification;

namespace CampusPlacementManagement.Interfaces;

public interface INotificationService
{
    Task<NotificationResponseDto> CreateAsync(
        NotificationCreateDto dto);

    Task<List<NotificationResponseDto>> GetMyNotificationsAsync(
        string userId);

    Task<int> GetUnreadCountAsync(string userId);

    Task MarkAsReadAsync(int id, string userId);

    Task MarkAllAsReadAsync(string userId);
}

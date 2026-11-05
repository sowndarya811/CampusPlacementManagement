
using CampusPlacementManagement.DTOs.Notification;
using CampusPlacementManagement.Interfaces;
using CampusPlacementManagement.Models;

namespace CampusPlacementManagement.Services;

public class NotificationService : INotificationService
{
    private readonly INotificationRepository _repository;

    public NotificationService(INotificationRepository repository)
    {
        _repository = repository;
    }

    // 1. Create Notification
    public async Task<NotificationResponseDto> CreateAsync(
        NotificationCreateDto dto)
    {
        var notification = new Notification
        {
            UserId = dto.UserId,
            Title = dto.Title,
            Message = dto.Message,
            Type = string.IsNullOrWhiteSpace(dto.Type)
                ? "General"
                : dto.Type,
            IsRead = false,
            CreatedAt = DateTime.UtcNow
        };

        await _repository.AddAsync(notification);

        return Map(notification);
    }

    // 2. Get Logged-in User Notifications
    public async Task<List<NotificationResponseDto>>
        GetMyNotificationsAsync(string userId)
    {
        var notifications = await _repository.GetByUserIdAsync(userId);

        return notifications.Select(Map).ToList();
    }

    // 3. Get Unread Notification Count
    public async Task<int> GetUnreadCountAsync(string userId)
    {
        return await _repository.GetUnreadCountAsync(userId);
    }

    // 4. Mark Individual Notification as Read
    public async Task MarkAsReadAsync(int id, string userId)
    {
        var notification = await _repository.GetByIdAsync(id);

        if (notification == null || notification.UserId != userId)
        {
            throw new KeyNotFoundException("Notification not found.");
        }

        if (!notification.IsRead)
        {
            notification.IsRead = true;
            notification.ReadAt = DateTime.UtcNow;

            await _repository.SaveChangesAsync();
        }
    }

    // 5. Mark All Notifications as Read
    public async Task MarkAllAsReadAsync(string userId)
    {
        var notifications = await _repository.GetUnreadByUserIdAsync(
            userId);

        foreach (var notification in notifications)
        {
            notification.IsRead = true;
            notification.ReadAt = DateTime.UtcNow;
        }

        if (notifications.Count > 0)
        {
            await _repository.SaveChangesAsync();
        }
    }

    // 6. Entity to Response DTO Mapping
    private static NotificationResponseDto Map(
        Notification notification)
    {
        return new NotificationResponseDto
        {
            NotificationId = notification.NotificationId,
            UserId = notification.UserId,
            Title = notification.Title,
            Message = notification.Message,
            Type = notification.Type,
            IsRead = notification.IsRead,
            CreatedAt = notification.CreatedAt,
            ReadAt = notification.ReadAt
        };
    }
}

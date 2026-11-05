
using System.Security.Claims;
using CampusPlacementManagement.DTOs.Notification;
using CampusPlacementManagement.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CampusPlacementManagement.Controllers;

[ApiController]
[Route("api/notifications")]
[Authorize]
public class NotificationsController : ControllerBase
{
    private readonly INotificationService _service;

    public NotificationsController(INotificationService service)
    {
        _service = service;
    }

    private string CurrentUserId =>
        User.FindFirstValue(ClaimTypes.NameIdentifier)
        ?? throw new UnauthorizedAccessException("User ID not found.");

    // 1. Admin Creates Notification
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(
        [FromBody] NotificationCreateDto dto)
    {
        var result = await _service.CreateAsync(dto);

        return Ok(result);
    }

    // 2. Get My Notifications
    [HttpGet("my")]
    public async Task<IActionResult> GetMyNotifications()
    {
        var result = await _service.GetMyNotificationsAsync(
            CurrentUserId);

        return Ok(result);
    }

    // 3. Get Unread Notification Count
    [HttpGet("my/unread-count")]
    public async Task<IActionResult> GetUnreadCount()
    {
        var count = await _service.GetUnreadCountAsync(
            CurrentUserId);

        return Ok(new { unreadCount = count });
    }

    // 4. Mark Individual Notification as Read
    [HttpPut("{id:int}/read")]
    public async Task<IActionResult> MarkAsRead(int id)
    {
        try
        {
            await _service.MarkAsReadAsync(id, CurrentUserId);

            return Ok(new
            {
                message = "Notification marked as read successfully."
            });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    // 5. Mark All Notifications as Read
    [HttpPut("my/read-all")]
    public async Task<IActionResult> MarkAllAsRead()
    {
        await _service.MarkAllAsReadAsync(CurrentUserId);

        return Ok(new
        {
            message = "All notifications marked as read successfully."
        });
    }
}

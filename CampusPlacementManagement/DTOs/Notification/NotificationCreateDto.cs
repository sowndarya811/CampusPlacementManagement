
using System.ComponentModel.DataAnnotations;

namespace CampusPlacementManagement.DTOs.Notification;

public class NotificationCreateDto
{
    [Required]
    public string UserId { get; set; } = string.Empty;

    [Required]
    [MaxLength(150)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [MaxLength(2000)]
    public string Message { get; set; } = string.Empty;

    [MaxLength(50)]
    public string Type { get; set; } = "General";
}

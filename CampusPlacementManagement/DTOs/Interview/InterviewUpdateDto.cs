using System.ComponentModel.DataAnnotations;

namespace CampusPlacementManagement.DTOs.Interview;

public class InterviewUpdateDto
{
    public DateTime InterviewDate { get; set; }

    [Required]
    [MaxLength(30)]
    public string InterviewTime { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string InterviewMode { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? MeetingLink { get; set; }

    [MaxLength(300)]
    public string? Location { get; set; }
}
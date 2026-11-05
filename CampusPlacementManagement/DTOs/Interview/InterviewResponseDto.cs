using CampusPlacementManagement.Enums;

namespace CampusPlacementManagement.DTOs.Interview;

public class InterviewResponseDto
{
    public int InterviewId { get; set; }

    public int ApplicationId { get; set; }

    public int StudentId { get; set; }

    public string RegisterNumber { get; set; } = string.Empty;

    public int PlacementId { get; set; }

    public string JobTitle { get; set; } = string.Empty;

    public string CompanyName { get; set; } = string.Empty;

    public int PlacementOfficerId { get; set; }

    public DateTime InterviewDate { get; set; }

    public string InterviewTime { get; set; } = string.Empty;

    public string InterviewMode { get; set; } = string.Empty;

    public string? MeetingLink { get; set; }

    public string? Location { get; set; }

    public string? Feedback { get; set; }

    public InterviewStatus Status { get; set; }

    public SelectionStatus SelectionStatus { get; set; }

    public DateTime CreatedAt { get; set; }
}
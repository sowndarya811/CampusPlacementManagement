using CampusPlacementManagement.Enums;

namespace CampusPlacementManagement.Models;

public class Interview
{
    public int InterviewId { get; set; }

    public int ApplicationId { get; set; }

    public int PlacementOfficerId { get; set; }

    public DateTime InterviewDate { get; set; }

    public string InterviewTime { get; set; } = string.Empty;

    public string InterviewMode { get; set; } = string.Empty;

    public string? MeetingLink { get; set; }

    public string? Location { get; set; }

    public string? Feedback { get; set; }

    public InterviewStatus Status { get; set; }
        = InterviewStatus.Scheduled;

    public SelectionStatus SelectionStatus { get; set; }
        = SelectionStatus.Pending;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    public PlacementApplication Application { get; set; } = null!;

    public PlacementOfficer PlacementOfficer { get; set; } = null!;
}
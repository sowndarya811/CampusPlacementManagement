
using CampusPlacementManagement.Enums;

namespace CampusPlacementManagement.Models;

public class PlacementApplication
{
    // Primary Key
    public int ApplicationId { get; set; }

    // Foreign Keys
    public int StudentId { get; set; }

    public int PlacementId { get; set; }

    // Application Details
    public string? CoverLetter { get; set; }

    public string? ResumeUrl { get; set; }

    // Application Status
    public ApplicationStatus Status { get; set; } =
        ApplicationStatus.Applied;

    // Timestamps
    public DateTime AppliedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    // Navigation Properties
    public Student Student { get; set; } = null!;

    public Placement Placement { get; set; } = null!;
}

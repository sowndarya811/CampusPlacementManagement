using CampusPlacementManagement.Enums;

namespace CampusPlacementManagement.DTOs.Application;

public class ApplicationResponseDto
{
    public int ApplicationId { get; set; }

    public int StudentId { get; set; }

    public string StudentName { get; set; } = string.Empty;
    public string StudentUserId { get; set; } = string.Empty;

    public string RegisterNumber { get; set; } = string.Empty;

    public int PlacementId { get; set; }

    public string JobTitle { get; set; } = string.Empty;

    public string CompanyName { get; set; } = string.Empty;

    public string? CoverLetter { get; set; }

    public string? ResumeUrl { get; set; }

    public ApplicationStatus Status { get; set; }

    public DateTime AppliedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}
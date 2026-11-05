using CampusPlacementManagement.Enums;

namespace CampusPlacementManagement.DTOs.Placement;

public class PlacementResponseDto
{
    public int PlacementId { get; set; }
    public int CompanyId { get; set; }
    public string CompanyName { get; set; } = string.Empty;

    public int CollegeId { get; set; }
    public string CollegeName { get; set; } = string.Empty;

    public int PlacementOfficerId { get; set; }

    public string JobTitle { get; set; } = string.Empty;
    public string JobDescription { get; set; } = string.Empty;
    public string JobLocation { get; set; } = string.Empty;
    public string EmploymentType { get; set; } = string.Empty;

    public decimal SalaryPackage { get; set; }
    public decimal MinimumCGPA { get; set; }

    public string EligibleDepartments { get; set; } = string.Empty;
    public int EligibleGraduationYear { get; set; }

    public DateTime ApplicationDeadline { get; set; }
    public DateTime? DriveDate { get; set; }

    public PlacementStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
}
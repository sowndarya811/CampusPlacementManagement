using CampusPlacementManagement.Enums;

namespace CampusPlacementManagement.Models;

public class Placement
{
    public int PlacementId { get; set; }

    public int CompanyId { get; set; }
    public int CollegeId { get; set; }
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

    public PlacementStatus Status { get; set; } = PlacementStatus.Open;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Company Company { get; set; } = null!;
    public College College { get; set; } = null!;
    public PlacementOfficer PlacementOfficer { get; set; } = null!;
}
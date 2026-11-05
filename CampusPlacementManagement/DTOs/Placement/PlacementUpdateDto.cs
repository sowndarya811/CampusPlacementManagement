using System.ComponentModel.DataAnnotations;

namespace CampusPlacementManagement.DTOs.Placement;

public class PlacementUpdateDto
{
    [Required, MaxLength(150)]
    public string JobTitle { get; set; } = string.Empty;

    [Required]
    public string JobDescription { get; set; } = string.Empty;

    [Required, MaxLength(150)]
    public string JobLocation { get; set; } = string.Empty;

    [Required, MaxLength(50)]
    public string EmploymentType { get; set; } = string.Empty;

    [Range(0, 1000)]
    public decimal SalaryPackage { get; set; }

    [Range(0, 10)]
    public decimal MinimumCGPA { get; set; }

    [Required]
    public string EligibleDepartments { get; set; } = string.Empty;

    [Range(2020, 2100)]
    public int EligibleGraduationYear { get; set; }

    public DateTime ApplicationDeadline { get; set; }

    public DateTime? DriveDate { get; set; }
}
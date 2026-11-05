using System.ComponentModel.DataAnnotations;

namespace CampusPlacementManagement.DTOs.Student;

public class StudentUpdateDto
{
    [Required]
    [StringLength(100)]
    public string FullName { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string RegisterNumber { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string Department { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string Degree { get; set; } = string.Empty;

    [Range(2020, 2100)]
    public int GraduationYear { get; set; }

    [Range(typeof(decimal), "0", "10")]
    public decimal CGPA { get; set; }

    [Range(0, 100)]
    public int ActiveBacklogs { get; set; }

    [Url]
    [StringLength(500)]
    public string? ResumeUrl { get; set; }
}
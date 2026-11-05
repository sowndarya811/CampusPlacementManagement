using System.ComponentModel.DataAnnotations;

namespace CampusPlacementManagement.DTOs.Application;

public class ApplicationCreateDto
{
    [Range(1, int.MaxValue)]
    public int PlacementId { get; set; }

    [MaxLength(2000)]
    public string? CoverLetter { get; set; }
}
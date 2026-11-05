using CampusPlacementManagement.Enums;
using System.ComponentModel.DataAnnotations;

namespace CampusPlacementManagement.DTOs.Application;

public class ApplicationStatusUpdateDto
{
    [Required]
    [EnumDataType(typeof(ApplicationStatus))]
    public ApplicationStatus Status { get; set; }
}
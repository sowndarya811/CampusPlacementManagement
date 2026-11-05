using CampusPlacementManagement.Enums;
using System.ComponentModel.DataAnnotations;

namespace CampusPlacementManagement.DTOs.Interview;

public class SelectionUpdateDto
{
    [EnumDataType(typeof(SelectionStatus))]
    public SelectionStatus SelectionStatus { get; set; }

    [MaxLength(2000)]
    public string? Feedback { get; set; }
}
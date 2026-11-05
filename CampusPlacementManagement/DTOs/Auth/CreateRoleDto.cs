
using System.ComponentModel.DataAnnotations;

namespace CampusPlacementManagement.DTOs.Auth;

public class CreateRoleDto
{
    [Required]
    [StringLength(50)]
    public string RoleName { get; set; } = string.Empty;
}


namespace CampusPlacementManagement.DTOs.Dashboard;

public class RecentPlacementDto
{
    public int PlacementId { get; set; }
    public string JobTitle { get; set; } = string.Empty;
    public string CompanyName { get; set; } = string.Empty;
    public string JobLocation { get; set; } = string.Empty;
    public decimal SalaryPackage { get; set; }
    public DateTime ApplicationDeadline { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Status { get; set; } = string.Empty;
}

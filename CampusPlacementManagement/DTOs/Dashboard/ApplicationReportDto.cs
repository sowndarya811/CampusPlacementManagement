
namespace CampusPlacementManagement.DTOs.Dashboard;

public class ApplicationReportDto
{
    public int TotalApplications { get; set; }
    public int Applied { get; set; }
    public int Shortlisted { get; set; }
    public int Rejected { get; set; }
    public int Withdrawn { get; set; }
}

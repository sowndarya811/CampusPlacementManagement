
namespace CampusPlacementManagement.DTOs.Dashboard;

public class InterviewReportDto
{
    public int TotalInterviews { get; set; }

    public int Scheduled { get; set; }
    public int Completed { get; set; }
    public int Cancelled { get; set; }

    public int PendingSelections { get; set; }
    public int Selected { get; set; }
    public int SelectionRejected { get; set; }
}


using CampusPlacementManagement.DTOs.Dashboard;

namespace CampusPlacementManagement.Interfaces;

public interface IDashboardService
{
    Task<DashboardSummaryDto> GetSummaryAsync();

    Task<ApplicationReportDto> GetApplicationReportAsync();

    Task<InterviewReportDto> GetInterviewReportAsync();

    Task<List<RecentPlacementDto>> GetRecentPlacementsAsync();
}

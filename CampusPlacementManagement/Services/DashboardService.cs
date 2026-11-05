
using CampusPlacementManagement.Data;
using CampusPlacementManagement.DTOs.Dashboard;
using CampusPlacementManagement.Enums;
using CampusPlacementManagement.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CampusPlacementManagement.Services;

public class DashboardService : IDashboardService
{
    private readonly ApplicationDbContext _context;

    public DashboardService(ApplicationDbContext context)
    {
        _context = context;
    }

    // 1. Overall Dashboard Summary
    public async Task<DashboardSummaryDto> GetSummaryAsync()
    {
        return new DashboardSummaryDto
        {
            TotalStudents = await _context.Students.CountAsync(),

            TotalCompanies = await _context.Companies.CountAsync(),

            TotalColleges = await _context.Colleges.CountAsync(),

            TotalPlacementOfficers =
                await _context.PlacementOfficers.CountAsync(),

            TotalPlacements = await _context.Placements.CountAsync(),

            TotalApplications = await _context.Applications.CountAsync(),

            TotalInterviews = await _context.Interviews.CountAsync(),

            TotalNotifications = await _context.Notifications.CountAsync()
        };
    }

    // 2. Application Status Report
    public async Task<ApplicationReportDto> GetApplicationReportAsync()
    {
        var applications = _context.Applications;

        return new ApplicationReportDto
        {
            TotalApplications = await applications.CountAsync(),

            Applied = await applications.CountAsync(
                a => a.Status == ApplicationStatus.Applied),

            Shortlisted = await applications.CountAsync(
                a => a.Status == ApplicationStatus.Shortlisted),

            Rejected = await applications.CountAsync(
                a => a.Status == ApplicationStatus.Rejected),

            Withdrawn = await applications.CountAsync(
                a => a.Status == ApplicationStatus.Withdrawn)
        };
    }

    // 3. Interview and Selection Report
    public async Task<InterviewReportDto> GetInterviewReportAsync()
    {
        var interviews = _context.Interviews;

        return new InterviewReportDto
        {
            TotalInterviews = await interviews.CountAsync(),

            Scheduled = await interviews.CountAsync(
                i => i.Status == InterviewStatus.Scheduled),

            Completed = await interviews.CountAsync(
                i => i.Status == InterviewStatus.Completed),

            Cancelled = await interviews.CountAsync(
                i => i.Status == InterviewStatus.Cancelled),

            PendingSelections = await interviews.CountAsync(
                i => i.SelectionStatus == SelectionStatus.Pending),

            Selected = await interviews.CountAsync(
                i => i.SelectionStatus == SelectionStatus.Selected),

            SelectionRejected = await interviews.CountAsync(
                i => i.SelectionStatus == SelectionStatus.Rejected)
        };
    }

    // 4. Recent Placement Report
    public async Task<List<RecentPlacementDto>> GetRecentPlacementsAsync()
    {
        return await _context.Placements
            .AsNoTracking()
            .OrderByDescending(p => p.CreatedAt)
            .Take(10)
            .Select(p => new RecentPlacementDto
            {
                PlacementId = p.PlacementId,
                JobTitle = p.JobTitle,
                CompanyName = p.Company.CompanyName,
                JobLocation = p.JobLocation,
                SalaryPackage = p.SalaryPackage,
                ApplicationDeadline = p.ApplicationDeadline,
                CreatedAt = p.CreatedAt,
                Status = p.Status.ToString()
            })
            .ToListAsync();
    }
}

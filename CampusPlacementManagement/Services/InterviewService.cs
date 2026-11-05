
using CampusPlacementManagement.Data;
using CampusPlacementManagement.DTOs.Interview;
using CampusPlacementManagement.Enums;
using CampusPlacementManagement.Interfaces;
using CampusPlacementManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace CampusPlacementManagement.Services;

public class InterviewService : IInterviewService
{
    private readonly IInterviewRepository _repository;
    private readonly IPlacementOfficerRepository _officerRepository;
    private readonly ApplicationDbContext _context;

    public InterviewService(
        IInterviewRepository repository,
        IPlacementOfficerRepository officerRepository,
        ApplicationDbContext context)
    {
        _repository = repository;
        _officerRepository = officerRepository;
        _context = context;
    }

    // 1. Schedule Interview
    public async Task<InterviewResponseDto> ScheduleAsync(
        InterviewCreateDto dto, string userId)
    {
        var officer = await GetApprovedOfficerAsync(userId);

        var application = await _context.Applications
            .Include(a => a.Student)
            .Include(a => a.Placement)
                .ThenInclude(p => p.Company)
            .FirstOrDefaultAsync(a =>
                a.ApplicationId == dto.ApplicationId);

        if (application == null ||
            application.Placement.CollegeId != officer.CollegeId)
        {
            throw new KeyNotFoundException("Application not found.");
        }

        if (application.Status != ApplicationStatus.Shortlisted)
        {
            throw new InvalidOperationException(
                "Only shortlisted students can be scheduled for interviews.");
        }

        if (dto.InterviewDate <= DateTime.UtcNow)
        {
            throw new InvalidOperationException(
                "Interview date must be in the future.");
        }

        if (await _repository.HasScheduledInterviewAsync(dto.ApplicationId))
        {
            throw new InvalidOperationException(
                "An active interview is already scheduled for this application.");
        }

        var interview = new Interview
        {
            ApplicationId = application.ApplicationId,
            PlacementOfficerId = officer.PlacementOfficerId,
            InterviewDate = dto.InterviewDate,
            InterviewTime = dto.InterviewTime,
            InterviewMode = dto.InterviewMode,
            MeetingLink = dto.MeetingLink,
            Location = dto.Location,
            Status = InterviewStatus.Scheduled,
            SelectionStatus = SelectionStatus.Pending,
            CreatedAt = DateTime.UtcNow,
            Application = application
        };

        await _repository.AddAsync(interview);

        return Map(interview);
    }

    // 2. Placement Officer Views Own Interviews
    public async Task<List<InterviewResponseDto>> GetMyInterviewsAsync(
        string userId)
    {
        var officer = await GetApprovedOfficerAsync(userId);

        var interviews = await _repository.GetByPlacementOfficerAsync(
            officer.PlacementOfficerId);

        return interviews.Select(Map).ToList();
    }

    // 3. Student Views Own Interviews
    public async Task<List<InterviewResponseDto>> GetStudentInterviewsAsync(
        string userId)
    {
        var student = await _context.Students
            .FirstOrDefaultAsync(s => s.UserId == userId);

        if (student == null)
        {
            throw new KeyNotFoundException("Student profile not found.");
        }

        var interviews = await _repository.GetByStudentAsync(
            student.StudentId);

        return interviews.Select(Map).ToList();
    }

    // 4. Update Interview Schedule
    public async Task<InterviewResponseDto> UpdateAsync(
        int id, InterviewUpdateDto dto, string userId)
    {
        var officer = await GetApprovedOfficerAsync(userId);

        var interview = await _repository.GetByIdAsync(id);

        if (interview == null ||
            interview.PlacementOfficerId != officer.PlacementOfficerId)
        {
            throw new KeyNotFoundException("Interview not found.");
        }

        if (interview.Status != InterviewStatus.Scheduled)
        {
            throw new InvalidOperationException(
                "Only scheduled interviews can be updated.");
        }

        if (dto.InterviewDate <= DateTime.UtcNow)
        {
            throw new InvalidOperationException(
                "Interview date must be in the future.");
        }

        interview.InterviewDate = dto.InterviewDate;
        interview.InterviewTime = dto.InterviewTime;
        interview.InterviewMode = dto.InterviewMode;
        interview.MeetingLink = dto.MeetingLink;
        interview.Location = dto.Location;
        interview.UpdatedAt = DateTime.UtcNow;

        await _repository.SaveChangesAsync();

        return Map(interview);
    }

    // 5. Complete Interview
    public async Task CompleteAsync(int id, string userId)
    {
        var officer = await GetApprovedOfficerAsync(userId);

        var interview = await _repository.GetByIdAsync(id);

        if (interview == null ||
            interview.PlacementOfficerId != officer.PlacementOfficerId)
        {
            throw new KeyNotFoundException("Interview not found.");
        }

        if (interview.Status != InterviewStatus.Scheduled)
        {
            throw new InvalidOperationException(
                "Only scheduled interviews can be completed.");
        }

        interview.Status = InterviewStatus.Completed;
        interview.UpdatedAt = DateTime.UtcNow;

        await _repository.SaveChangesAsync();
    }

    // 6. Update Selection Result
    public async Task<InterviewResponseDto> UpdateSelectionAsync(
        int id, SelectionUpdateDto dto, string userId)
    {
        var officer = await GetApprovedOfficerAsync(userId);

        var interview = await _repository.GetByIdAsync(id);

        if (interview == null ||
            interview.PlacementOfficerId != officer.PlacementOfficerId)
        {
            throw new KeyNotFoundException("Interview not found.");
        }

        if (interview.Status != InterviewStatus.Completed)
        {
            throw new InvalidOperationException(
                "Complete the interview before updating selection status.");
        }

        if (dto.SelectionStatus == SelectionStatus.Pending)
        {
            throw new InvalidOperationException(
                "Selection status must be Selected or Rejected.");
        }

        interview.SelectionStatus = dto.SelectionStatus;
        interview.Feedback = dto.Feedback;
        interview.UpdatedAt = DateTime.UtcNow;

        await _repository.SaveChangesAsync();

        return Map(interview);
    }

    // 7. Get Approved Placement Officer
    private async Task<PlacementOfficer> GetApprovedOfficerAsync(
        string userId)
    {
        var officer = await _officerRepository.GetByUserIdAsync(userId);

        if (officer == null ||
            officer.Status != PlacementOfficerStatus.Approved)
        {
            throw new InvalidOperationException(
                "Approved placement officer not found.");
        }

        return officer;
    }

    // 8. Map Interview Entity to Response DTO
    private static InterviewResponseDto Map(Interview interview)
    {
        var application = interview.Application;
        var student = application.Student;
        var placement = application.Placement;

        return new InterviewResponseDto
        {
            InterviewId = interview.InterviewId,
            ApplicationId = interview.ApplicationId,
            StudentId = student.StudentId,
            RegisterNumber = student.RegisterNumber,
            PlacementId = placement.PlacementId,
            JobTitle = placement.JobTitle,
            CompanyName = placement.Company.CompanyName,
            PlacementOfficerId = interview.PlacementOfficerId,
            InterviewDate = interview.InterviewDate,
            InterviewTime = interview.InterviewTime,
            InterviewMode = interview.InterviewMode,
            MeetingLink = interview.MeetingLink,
            Location = interview.Location,
            Feedback = interview.Feedback,
            Status = interview.Status,
            SelectionStatus = interview.SelectionStatus,
            CreatedAt = interview.CreatedAt
        };
    }
}

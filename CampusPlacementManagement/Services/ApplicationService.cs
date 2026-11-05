using CampusPlacementManagement.Data;
using CampusPlacementManagement.DTOs.Application;
using CampusPlacementManagement.Enums;
using CampusPlacementManagement.Interfaces;
using CampusPlacementManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace CampusPlacementManagement.Services;

public class ApplicationService : IApplicationService
{
    private readonly IApplicationRepository _repository;
    private readonly ApplicationDbContext _context;
    private readonly IPlacementOfficerRepository _officerRepository;

    public ApplicationService(
        IApplicationRepository repository,
        ApplicationDbContext context,
        IPlacementOfficerRepository officerRepository)
    {
        _repository = repository;
        _context = context;
        _officerRepository = officerRepository;
    }

    // Student applies for a placement
    public async Task<ApplicationResponseDto> ApplyAsync(
        ApplicationCreateDto dto, string userId)
    {
        var student = await _context.Students
            .FirstOrDefaultAsync(s => s.UserId == userId);

        if (student == null)
            throw new KeyNotFoundException("Student profile not found.");

        var placement = await _context.Placements
            .Include(p => p.Company)
            .FirstOrDefaultAsync(p => p.PlacementId == dto.PlacementId);

        if (placement == null)
            throw new KeyNotFoundException("Placement not found.");

        if (placement.Status != PlacementStatus.Open)
            throw new InvalidOperationException("This placement is not open.");

        if (placement.ApplicationDeadline <= DateTime.UtcNow)
            throw new InvalidOperationException(
                "Application deadline has passed.");

        if (student.CollegeId != placement.CollegeId)
            throw new InvalidOperationException(
                "You can only apply for placements from your college.");

        if (student.CGPA < placement.MinimumCGPA)
            throw new InvalidOperationException(
                "Your CGPA does not meet the eligibility criteria.");

        if (!placement.EligibleDepartments
            .Split(',', StringSplitOptions.TrimEntries |
                         StringSplitOptions.RemoveEmptyEntries)
            .Contains(student.Department,
                StringComparer.OrdinalIgnoreCase))
            throw new InvalidOperationException(
                "Your department is not eligible for this placement.");

        if (await _repository.ExistsAsync(
            student.StudentId, placement.PlacementId))
            throw new InvalidOperationException(
                "You have already applied for this placement.");

        var application = new PlacementApplication
        {
            StudentId = student.StudentId,
            PlacementId = placement.PlacementId,
            CoverLetter = dto.CoverLetter,
            ResumeUrl = student.ResumeUrl,
            Status = ApplicationStatus.Applied,
            AppliedAt = DateTime.UtcNow
        };

        await _repository.AddAsync(application);

        return Map(application, student, placement);
    }

    // Student views their own applications
    public async Task<List<ApplicationResponseDto>> GetMyApplicationsAsync(
        string userId)
    {
        var student = await _context.Students
            .FirstOrDefaultAsync(s => s.UserId == userId);

        if (student == null)
            throw new KeyNotFoundException("Student profile not found.");

        var applications = await _repository.GetByStudentIdAsync(
            student.StudentId);

        return applications.Select(a =>
            Map(a, a.Student, a.Placement)).ToList();
    }

    // Student withdraws an application
    public async Task WithdrawAsync(int applicationId, string userId)
    {
        var student = await _context.Students
            .FirstOrDefaultAsync(s => s.UserId == userId);

        if (student == null)
            throw new KeyNotFoundException("Student profile not found.");

        var application = await _repository.GetByIdAsync(applicationId);

        if (application == null || application.StudentId != student.StudentId)
            throw new KeyNotFoundException("Application not found.");

        if (application.Status != ApplicationStatus.Applied)
            throw new InvalidOperationException(
                "Only applications with Applied status can be withdrawn.");

        application.Status = ApplicationStatus.Withdrawn;
        application.UpdatedAt = DateTime.UtcNow;

        await _repository.SaveChangesAsync();
    }

    // Placement Officer views applicants for their college placement
    public async Task<List<ApplicationResponseDto>> GetApplicantsAsync(
        int placementId, string userId)
    {
        var officer = await GetApprovedOfficerAsync(userId);

        var placement = await _context.Placements
            .FirstOrDefaultAsync(p => p.PlacementId == placementId);

        if (placement == null || placement.CollegeId != officer.CollegeId)
            throw new KeyNotFoundException("Placement not found.");

        var applications = await _repository.GetByPlacementIdAsync(placementId);

        return applications.Select(a =>
            Map(a, a.Student, a.Placement)).ToList();
    }

    // Placement Officer shortlists or rejects an applicant
    public async Task<ApplicationResponseDto> UpdateStatusAsync(
        int applicationId,
        ApplicationStatusUpdateDto dto,
        string userId)
    {
        var officer = await GetApprovedOfficerAsync(userId);

        if (dto.Status != ApplicationStatus.Shortlisted &&
            dto.Status != ApplicationStatus.Rejected)
            throw new InvalidOperationException(
                "Only Shortlisted or Rejected status is allowed.");

        var application = await _repository.GetByIdAsync(applicationId);

        if (application == null ||
            application.Placement.CollegeId != officer.CollegeId)
            throw new KeyNotFoundException("Application not found.");

        if (application.Status != ApplicationStatus.Applied)
            throw new InvalidOperationException(
                "Only Applied applications can be reviewed.");

        application.Status = dto.Status;
        application.UpdatedAt = DateTime.UtcNow;

        await _repository.SaveChangesAsync();

        return Map(application, application.Student, application.Placement);
    }

    private async Task<PlacementOfficer> GetApprovedOfficerAsync(
        string userId)
    {
        var officer = await _officerRepository.GetByUserIdAsync(userId);

        if (officer == null ||
            officer.Status != PlacementOfficerStatus.Approved)
            throw new InvalidOperationException(
                "Approved placement officer not found.");

        return officer;
    }

    private static ApplicationResponseDto Map(
        PlacementApplication application,
        Student student,
        Placement placement)
    {
        return new ApplicationResponseDto
        {
            ApplicationId = application.ApplicationId,
            StudentId = student.StudentId,
            StudentName = student.UserId,
            StudentUserId = student.UserId,
            RegisterNumber = student.RegisterNumber,
            PlacementId = placement.PlacementId,
            JobTitle = placement.JobTitle,
            CompanyName = placement.Company.CompanyName,
            CoverLetter = application.CoverLetter,
            ResumeUrl = application.ResumeUrl,
            Status = application.Status,
            AppliedAt = application.AppliedAt,
            UpdatedAt = application.UpdatedAt
        };
    }
}
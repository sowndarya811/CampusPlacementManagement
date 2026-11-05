using CampusPlacementManagement.Data;
using CampusPlacementManagement.DTOs.Placement;
using CampusPlacementManagement.Enums;
using CampusPlacementManagement.Interfaces;
using CampusPlacementManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace CampusPlacementManagement.Services;

public class PlacementService : IPlacementService
{
    private readonly IPlacementRepository _repository;
    private readonly IPlacementOfficerRepository _officerRepository;
    private readonly ApplicationDbContext _context;

    public PlacementService(
        IPlacementRepository repository,
        IPlacementOfficerRepository officerRepository,
        ApplicationDbContext context)
    {
        _repository = repository;
        _officerRepository = officerRepository;
        _context = context;
    }

    public async Task<PlacementResponseDto> CreateAsync(
        PlacementCreateDto dto, string userId)
    {
        var officer = await _officerRepository.GetByUserIdAsync(userId);

        if (officer == null ||
            officer.Status != PlacementOfficerStatus.Approved)
            throw new InvalidOperationException(
                "Only approved placement officers can create placements.");

        var companyExists = await _context.Companies
            .AnyAsync(c => c.CompanyId == dto.CompanyId);

        if (!companyExists)
            throw new KeyNotFoundException("Company not found.");

        if (dto.ApplicationDeadline <= DateTime.UtcNow)
            throw new InvalidOperationException(
                "Application deadline must be in the future.");

        if (dto.DriveDate.HasValue &&
            dto.DriveDate.Value < dto.ApplicationDeadline)
            throw new InvalidOperationException(
                "Drive date must be on or after the application deadline.");

        var placement = new Placement
        {
            CompanyId = dto.CompanyId,
            CollegeId = officer.CollegeId,
            PlacementOfficerId = officer.PlacementOfficerId,
            JobTitle = dto.JobTitle,
            JobDescription = dto.JobDescription,
            JobLocation = dto.JobLocation,
            EmploymentType = dto.EmploymentType,
            SalaryPackage = dto.SalaryPackage,
            MinimumCGPA = dto.MinimumCGPA,
            EligibleDepartments = dto.EligibleDepartments,
            EligibleGraduationYear = dto.EligibleGraduationYear,
            ApplicationDeadline = dto.ApplicationDeadline,
            DriveDate = dto.DriveDate,
            Status = PlacementStatus.Open,
            CreatedAt = DateTime.UtcNow
        };

        await _repository.AddAsync(placement);

        return Map(placement, officer.College.CollegeName,
            (await _context.Companies.FindAsync(dto.CompanyId))!.CompanyName);
    }

    public async Task<List<PlacementResponseDto>> GetAllAsync(string userId)
    {
        var officer = await _officerRepository.GetByUserIdAsync(userId);

        if (officer == null ||
            officer.Status != PlacementOfficerStatus.Approved)
            throw new InvalidOperationException("Approved officer not found.");

        var placements = await _repository.GetByCollegeIdAsync(officer.CollegeId);

        return placements.Select(p => Map(
            p, p.College.CollegeName, p.Company.CompanyName)).ToList();
    }

    public async Task<PlacementResponseDto> GetByIdAsync(
        int id, string userId)
    {
        var officer = await GetApprovedOfficer(userId);
        var placement = await GetPlacementForCollege(id, officer.CollegeId);

        return Map(placement, placement.College.CollegeName,
            placement.Company.CompanyName);
    }

    public async Task<PlacementResponseDto> UpdateAsync(
        int id, PlacementUpdateDto dto, string userId)
    {
        var officer = await GetApprovedOfficer(userId);
        var placement = await GetPlacementForCollege(id, officer.CollegeId);

        if (placement.Status != PlacementStatus.Open)
            throw new InvalidOperationException(
                "Only open placements can be updated.");

        if (dto.ApplicationDeadline <= DateTime.UtcNow)
            throw new InvalidOperationException(
                "Application deadline must be in the future.");

        if (dto.DriveDate.HasValue &&
            dto.DriveDate.Value < dto.ApplicationDeadline)
            throw new InvalidOperationException(
                "Drive date must be on or after the application deadline.");

        placement.JobTitle = dto.JobTitle;
        placement.JobDescription = dto.JobDescription;
        placement.JobLocation = dto.JobLocation;
        placement.EmploymentType = dto.EmploymentType;
        placement.SalaryPackage = dto.SalaryPackage;
        placement.MinimumCGPA = dto.MinimumCGPA;
        placement.EligibleDepartments = dto.EligibleDepartments;
        placement.EligibleGraduationYear = dto.EligibleGraduationYear;
        placement.ApplicationDeadline = dto.ApplicationDeadline;
        placement.DriveDate = dto.DriveDate;

        await _repository.SaveChangesAsync();

        return Map(placement, placement.College.CollegeName,
            placement.Company.CompanyName);
    }

    public async Task CloseAsync(int id, string userId)
    {
        var officer = await GetApprovedOfficer(userId);
        var placement = await GetPlacementForCollege(id, officer.CollegeId);

        if (placement.Status != PlacementStatus.Open)
            throw new InvalidOperationException(
                "Only open placements can be closed.");

        placement.Status = PlacementStatus.Closed;
        await _repository.SaveChangesAsync();
    }

    private async Task<PlacementOfficer> GetApprovedOfficer(string userId)
    {
        var officer = await _officerRepository.GetByUserIdAsync(userId);

        if (officer == null ||
            officer.Status != PlacementOfficerStatus.Approved)
            throw new InvalidOperationException(
                "Approved placement officer not found.");

        return officer;
    }

    private async Task<Placement> GetPlacementForCollege(
        int id, int collegeId)
    {
        var placement = await _repository.GetByIdAsync(id);

        if (placement == null || placement.CollegeId != collegeId)
            throw new KeyNotFoundException("Placement not found.");

        return placement;
    }

    private static PlacementResponseDto Map(
        Placement p, string collegeName, string companyName)
    {
        return new PlacementResponseDto
        {
            PlacementId = p.PlacementId,
            CompanyId = p.CompanyId,
            CompanyName = companyName,
            CollegeId = p.CollegeId,
            CollegeName = collegeName,
            PlacementOfficerId = p.PlacementOfficerId,
            JobTitle = p.JobTitle,
            JobDescription = p.JobDescription,
            JobLocation = p.JobLocation,
            EmploymentType = p.EmploymentType,
            SalaryPackage = p.SalaryPackage,
            MinimumCGPA = p.MinimumCGPA,
            EligibleDepartments = p.EligibleDepartments,
            EligibleGraduationYear = p.EligibleGraduationYear,
            ApplicationDeadline = p.ApplicationDeadline,
            DriveDate = p.DriveDate,
            Status = p.Status,
            CreatedAt = p.CreatedAt
        };
    }
}
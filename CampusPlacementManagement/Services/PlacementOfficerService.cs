using CampusPlacementManagement.Data;
using CampusPlacementManagement.DTOs.PlacementOfficer;
using CampusPlacementManagement.Enums;
using CampusPlacementManagement.Interfaces;
using CampusPlacementManagement.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CampusPlacementManagement.Services;

public class PlacementOfficerService : IPlacementOfficerService
{
    private readonly IPlacementOfficerRepository _repository;
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public PlacementOfficerService(
        IPlacementOfficerRepository repository,
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager)
    {
        _repository = repository;
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task<PlacementOfficerResponseDto> RegisterAsync(
        PlacementOfficerRegisterDto dto)
    {
        var email = dto.Email.Trim();

        if (await _userManager.FindByEmailAsync(email) != null)
            throw new InvalidOperationException("Email already exists.");

        var college = await _context.Colleges
            .FirstOrDefaultAsync(x => x.CollegeId == dto.CollegeId);

        if (college == null)
            throw new KeyNotFoundException("College not found.");

        if (college.Status != CollegeStatus.Approved)
            throw new InvalidOperationException(
                "Only approved colleges can register placement officers.");

        var user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            FullName = dto.FullName.Trim(),
            PhoneNumber = dto.PhoneNumber.Trim(),
            IsActive = false
        };

        var result = await _userManager.CreateAsync(user, dto.Password);

        if (!result.Succeeded)
        {
            var errors = string.Join("; ", result.Errors.Select(x => x.Description));
            throw new InvalidOperationException(errors);
        }

        var officer = new PlacementOfficer
        {
            UserId = user.Id,
            CollegeId = dto.CollegeId,
            Designation = dto.Designation.Trim(),
            Status = PlacementOfficerStatus.Pending
        };

        try
        {
            await _repository.AddAsync(officer);
        }
        catch
        {
            await _userManager.DeleteAsync(user);
            throw;
        }

        return Map(officer, user, college.CollegeName);
    }

    public async Task<List<PlacementOfficerResponseDto>> GetAllAsync()
    {
        var officers = await _repository.GetAllAsync();
        return officers.Select(x => Map(x)).ToList();
    }

    public async Task<List<PlacementOfficerResponseDto>> GetPendingAsync()
    {
        var officers = await _repository.GetPendingAsync();
        return officers.Select(x => Map(x)).ToList();
    }

    public async Task<PlacementOfficerResponseDto?> GetByIdAsync(int id)
    {
        var officer = await _repository.GetByIdAsync(id);
        return officer == null ? null : Map(officer);
    }

    public async Task<PlacementOfficerResponseDto?> GetMyProfileAsync(string userId)
    {
        var officer = await _repository.GetByUserIdAsync(userId);
        return officer == null ? null : Map(officer);
    }

    public async Task<bool> ApproveAsync(int id, string adminUserId)
    {
        var officer = await _repository.GetByIdAsync(id);

        if (officer == null)
            return false;

        if (officer.Status != PlacementOfficerStatus.Pending)
            throw new InvalidOperationException(
                "Only pending placement officers can be approved.");

        if (officer.College.Status != CollegeStatus.Approved)
            throw new InvalidOperationException(
                "The associated college is not approved.");

        var roleExists = await _roleManager.RoleExistsAsync("PlacementOfficer");

        if (!roleExists)
        {
            var roleResult = await _roleManager.CreateAsync(
                new IdentityRole("PlacementOfficer"));

            if (!roleResult.Succeeded)
                throw new InvalidOperationException("Unable to create PlacementOfficer role.");
        }

        var roleResultAdd = await _userManager.AddToRoleAsync(
            officer.User, "PlacementOfficer");

        if (!roleResultAdd.Succeeded)
        {
            var errors = string.Join("; ", roleResultAdd.Errors.Select(x => x.Description));
            throw new InvalidOperationException(errors);
        }

        officer.Status = PlacementOfficerStatus.Approved;
        officer.ApprovedBy = adminUserId;
        officer.ApprovedAt = DateTime.UtcNow;
        officer.User.IsActive = true;

        await _repository.SaveChangesAsync();

        return true;
    }

    public async Task<bool> RejectAsync(int id)
    {
        var officer = await _repository.GetByIdAsync(id);

        if (officer == null)
            return false;

        if (officer.Status != PlacementOfficerStatus.Pending)
            throw new InvalidOperationException(
                "Only pending placement officers can be rejected.");

        officer.Status = PlacementOfficerStatus.Rejected;
        officer.User.IsActive = false;

        await _repository.SaveChangesAsync();
        return true;
    }

    public async Task<bool> SuspendAsync(int id)
    {
        var officer = await _repository.GetByIdAsync(id);

        if (officer == null)
            return false;

        if (officer.Status != PlacementOfficerStatus.Approved)
            throw new InvalidOperationException(
                "Only approved placement officers can be suspended.");

        officer.Status = PlacementOfficerStatus.Suspended;
        officer.User.IsActive = false;

        await _repository.SaveChangesAsync();
        return true;
    }

    private static PlacementOfficerResponseDto Map(
        PlacementOfficer officer,
        ApplicationUser? user = null,
        string? collegeName = null)
    {
        user ??= officer.User;

        return new PlacementOfficerResponseDto
        {
            PlacementOfficerId = officer.PlacementOfficerId,
            UserId = officer.UserId,
            FullName = user.FullName,
            Email = user.Email ?? string.Empty,
            PhoneNumber = user.PhoneNumber ?? string.Empty,
            CollegeId = officer.CollegeId,
            CollegeName = collegeName ?? officer.College?.CollegeName ?? string.Empty,
            Designation = officer.Designation,
            Status = officer.Status,
            ApprovedBy = officer.ApprovedBy,
            ApprovedAt = officer.ApprovedAt
        };
    }
}
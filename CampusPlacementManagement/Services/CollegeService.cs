using CampusPlacementManagement.DTOs.College;
using CampusPlacementManagement.Enums;
using CampusPlacementManagement.Interfaces;
using CampusPlacementManagement.Models;

namespace CampusPlacementManagement.Services;

public class CollegeService : ICollegeService
{
    private readonly ICollegeRepository _repository;

    public CollegeService(ICollegeRepository repository)
    {
        _repository = repository;
    }

    public async Task<CollegeResponseDto> RegisterAsync(
        CollegeCreateDto request)
    {
        if (await _repository.CodeExistsAsync(request.CollegeCode))
            throw new InvalidOperationException(
                "College code already exists.");

        if (await _repository.EmailExistsAsync(request.OfficialEmail))
            throw new InvalidOperationException(
                "College email already exists.");

        var college = new College
        {
            CollegeName = request.CollegeName.Trim(),
            CollegeCode = request.CollegeCode.Trim().ToUpperInvariant(),
            UniversityName = request.UniversityName,
            OfficialEmail = request.OfficialEmail.Trim().ToLowerInvariant(),
            PhoneNumber = request.PhoneNumber,
            Address = request.Address,
            Website = request.Website,
            Status = CollegeStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };

        await _repository.AddAsync(college);

        return MapToResponse(college);
    }

    public async Task<List<CollegeResponseDto>> GetAllAsync()
    {
        var colleges = await _repository.GetAllAsync();

        return colleges.Select(MapToResponse).ToList();
    }

    public async Task<List<CollegeResponseDto>> GetPendingAsync()
    {
        var colleges = await _repository.GetPendingAsync();

        return colleges.Select(MapToResponse).ToList();
    }

    public async Task<CollegeResponseDto> GetByIdAsync(int id)
    {
        var college = await _repository.GetByIdAsync(id);

        if (college == null)
            throw new KeyNotFoundException("College not found.");

        return MapToResponse(college);
    }

    public async Task<CollegeResponseDto> ApproveAsync(int id)
    {
        var college = await GetCollegeEntityAsync(id);

        if (college.Status != CollegeStatus.Pending)
            throw new InvalidOperationException(
                "Only pending colleges can be approved.");

        college.Status = CollegeStatus.Approved;

        await _repository.UpdateAsync(college);
        await _repository.SaveChangesAsync();

        return MapToResponse(college);
    }

    public async Task<CollegeResponseDto> RejectAsync(int id)
    {
        var college = await GetCollegeEntityAsync(id);

        if (college.Status != CollegeStatus.Pending)
            throw new InvalidOperationException(
                "Only pending colleges can be rejected.");

        college.Status = CollegeStatus.Rejected;

        await _repository.UpdateAsync(college);
        await _repository.SaveChangesAsync();

        return MapToResponse(college);
    }

    public async Task<CollegeResponseDto> SuspendAsync(int id)
    {
        var college = await GetCollegeEntityAsync(id);

        if (college.Status != CollegeStatus.Approved)
            throw new InvalidOperationException(
                "Only approved colleges can be suspended.");

        college.Status = CollegeStatus.Suspended;

        await _repository.UpdateAsync(college);
        await _repository.SaveChangesAsync();

        return MapToResponse(college);
    }

    public async Task<CollegeResponseDto> UpdateAsync(
        int id,
        CollegeUpdateDto request)
    {
        var college = await GetCollegeEntityAsync(id);

        if (await _repository.CodeExistsForOtherCollegeAsync(
            request.CollegeCode, id))
        {
            throw new InvalidOperationException(
                "College code already exists.");
        }

        college.CollegeName = request.CollegeName.Trim();
        college.CollegeCode = request.CollegeCode.Trim().ToUpperInvariant();
        college.UniversityName = request.UniversityName;
        college.OfficialEmail = request.OfficialEmail.Trim().ToLowerInvariant();
        college.PhoneNumber = request.PhoneNumber;
        college.Address = request.Address;
        college.Website = request.Website;

        await _repository.UpdateAsync(college);
        await _repository.SaveChangesAsync();

        return MapToResponse(college);
    }

    private async Task<College> GetCollegeEntityAsync(int id)
    {
        var college = await _repository.GetByIdAsync(id);

        if (college == null)
            throw new KeyNotFoundException("College not found.");

        return college;
    }

    private static CollegeResponseDto MapToResponse(College college)
    {
        return new CollegeResponseDto
        {
            CollegeId = college.CollegeId,
            CollegeName = college.CollegeName,
            CollegeCode = college.CollegeCode,
            UniversityName = college.UniversityName,
            OfficialEmail = college.OfficialEmail,
            PhoneNumber = college.PhoneNumber,
            Address = college.Address,
            Website = college.Website,
            Status = college.Status,
            CreatedAt = college.CreatedAt
        };
    }
}
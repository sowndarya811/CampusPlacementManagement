
using CampusPlacementManagement.DTOs.Company;
using CampusPlacementManagement.Enums;
using CampusPlacementManagement.Interfaces;
using CampusPlacementManagement.Models;

namespace CampusPlacementManagement.Services;

public class CompanyService : ICompanyService
{
    private readonly ICompanyRepository _companyRepository;

    public CompanyService(ICompanyRepository companyRepository)
    {
        _companyRepository = companyRepository;
    }

    // Register Company
    public async Task<CompanyResponseDto> RegisterCompanyAsync(
        CompanyRegisterDto request)
    {
        if (await _companyRepository.ExistsByEmailAsync(request.CompanyEmail))
        {
            throw new InvalidOperationException(
                "A company with this email already exists.");
        }

        var company = new Company
        {
            CompanyName = request.CompanyName,
            CompanyEmail = request.CompanyEmail,
            Website = request.Website,
            Industry = request.Industry,
            Address = request.Address,
            Description = request.Description,
            Status = CompanyStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };

        var savedCompany = await _companyRepository.AddAsync(company);

        return MapToResponseDto(savedCompany);
    }

    // Get Company By ID
    public async Task<CompanyResponseDto?> GetCompanyByIdAsync(int companyId)
    {
        var company = await _companyRepository.GetByIdAsync(companyId);

        return company == null ? null : MapToResponseDto(company);
    }

    // Get All Companies
    public async Task<List<CompanyResponseDto>> GetAllCompaniesAsync()
    {
        var companies = await _companyRepository.GetAllAsync();

        return companies.Select(MapToResponseDto).ToList();
    }

    // Get Pending Companies
    public async Task<List<CompanyResponseDto>> GetPendingCompaniesAsync()
    {
        var companies = await _companyRepository.GetPendingAsync();

        return companies.Select(MapToResponseDto).ToList();
    }

    // Update Company
    public async Task<CompanyResponseDto> UpdateCompanyAsync(
        int companyId,
        CompanyUpdateDto request)
    {
        var company = await _companyRepository.GetByIdAsync(companyId);

        if (company == null)
        {
            throw new KeyNotFoundException("Company not found.");
        }

        if (!string.Equals(company.CompanyEmail, request.CompanyEmail,
                StringComparison.OrdinalIgnoreCase) &&
            await _companyRepository.ExistsByEmailAsync(request.CompanyEmail))
        {
            throw new InvalidOperationException(
                "This email is already registered.");
        }

        company.CompanyName = request.CompanyName;
        company.CompanyEmail = request.CompanyEmail;
        company.Website = request.Website;
        company.Industry = request.Industry;
        company.Address = request.Address;
        company.Description = request.Description;

        await _companyRepository.UpdateAsync(company);

        return MapToResponseDto(company);
    }

    // Approve Company
    public async Task ApproveCompanyAsync(int companyId)
    {
        var company = await _companyRepository.GetByIdAsync(companyId);

        if (company == null)
        {
            throw new KeyNotFoundException("Company not found.");
        }

        if (company.Status != CompanyStatus.Pending)
        {
            throw new InvalidOperationException(
                "Only pending companies can be approved.");
        }

        company.Status = CompanyStatus.Approved;

        await _companyRepository.UpdateAsync(company);
    }

    // Reject Company
    public async Task RejectCompanyAsync(int companyId)
    {
        var company = await _companyRepository.GetByIdAsync(companyId);

        if (company == null)
        {
            throw new KeyNotFoundException("Company not found.");
        }

        if (company.Status != CompanyStatus.Pending)
        {
            throw new InvalidOperationException(
                "Only pending companies can be rejected.");
        }

        company.Status = CompanyStatus.Rejected;

        await _companyRepository.UpdateAsync(company);
    }

    // Entity to DTO Mapping
    private static CompanyResponseDto MapToResponseDto(Company company)
    {
        return new CompanyResponseDto
        {
            CompanyId = company.CompanyId,
            CompanyName = company.CompanyName,
            CompanyEmail = company.CompanyEmail,
            Website = company.Website,
            Industry = company.Industry,
            Address = company.Address,
            Description = company.Description,
            Status = company.Status,
            CreatedAt = company.CreatedAt
        };
    }
}

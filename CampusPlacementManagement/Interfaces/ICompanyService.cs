
using CampusPlacementManagement.DTOs.Company;

namespace CampusPlacementManagement.Interfaces;

public interface ICompanyService
{
    Task<CompanyResponseDto> RegisterCompanyAsync(
        CompanyRegisterDto request);

    Task<CompanyResponseDto?> GetCompanyByIdAsync(
        int companyId);

    Task<List<CompanyResponseDto>> GetAllCompaniesAsync();

    Task<List<CompanyResponseDto>> GetPendingCompaniesAsync();

    Task<CompanyResponseDto> UpdateCompanyAsync(
        int companyId,
        CompanyUpdateDto request);

    Task ApproveCompanyAsync(int companyId);

    Task RejectCompanyAsync(int companyId);
}

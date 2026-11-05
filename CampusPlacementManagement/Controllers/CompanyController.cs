
using CampusPlacementManagement.DTOs.Company;
using CampusPlacementManagement.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CampusPlacementManagement.Controllers;

[ApiController]
[Route("api/companies")]
public class CompanyController : ControllerBase
{
    private readonly ICompanyService _companyService;

    public CompanyController(ICompanyService companyService)
    {
        _companyService = companyService;
    }

    // Register Company
    [HttpPost("register")]
    public async Task<IActionResult> RegisterCompany(
        [FromBody] CompanyRegisterDto request)
    {
        var result = await _companyService.RegisterCompanyAsync(request);

        return CreatedAtAction(
            nameof(GetCompanyById),
            new { id = result.CompanyId },
            result);
    }

    // Get All Companies
    [HttpGet]
    public async Task<IActionResult> GetAllCompanies()
    {
        var companies = await _companyService.GetAllCompaniesAsync();

        return Ok(companies);
    }

    // Get Company By ID
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetCompanyById(int id)
    {
        var company = await _companyService.GetCompanyByIdAsync(id);

        if (company == null)
        {
            return NotFound(new { message = "Company not found." });
        }

        return Ok(company);
    }

    // Get Pending Companies
    [HttpGet("pending")]
    public async Task<IActionResult> GetPendingCompanies()
    {
        var companies = await _companyService.GetPendingCompaniesAsync();

        return Ok(companies);
    }

    // Update Company
    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateCompany(
        int id,
        [FromBody] CompanyUpdateDto request)
    {
        var result = await _companyService.UpdateCompanyAsync(id, request);

        return Ok(result);
    }

    // Approve Company
    [HttpPut("{id:int}/approve")]
    public async Task<IActionResult> ApproveCompany(int id)
    {
        await _companyService.ApproveCompanyAsync(id);

        return Ok(new { message = "Company approved successfully." });
    }

    // Reject Company
    [HttpPut("{id:int}/reject")]
    public async Task<IActionResult> RejectCompany(int id)
    {
        await _companyService.RejectCompanyAsync(id);

        return Ok(new { message = "Company rejected successfully." });
    }
}

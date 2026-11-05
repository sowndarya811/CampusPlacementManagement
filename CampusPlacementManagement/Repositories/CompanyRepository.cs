
using CampusPlacementManagement.Data;
using CampusPlacementManagement.Interfaces;
using CampusPlacementManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace CampusPlacementManagement.Repositories;

public class CompanyRepository : ICompanyRepository
{
    private readonly ApplicationDbContext _context;

    public CompanyRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    // Add new company
    public async Task<Company> AddAsync(Company company)
    {
        await _context.Companies.AddAsync(company);
        await _context.SaveChangesAsync();

        return company;
    }

    // Get company by ID
    public async Task<Company?> GetByIdAsync(int companyId)
    {
        return await _context.Companies
            .FirstOrDefaultAsync(c => c.CompanyId == companyId);
    }

    // Get all companies
    public async Task<List<Company>> GetAllAsync()
    {
        return await _context.Companies
            .AsNoTracking()
            .ToListAsync();
    }

    // Get pending companies
    public async Task<List<Company>> GetPendingAsync()
    {
        return await _context.Companies
            .AsNoTracking()
            .Where(c => c.Status == Enums.CompanyStatus.Pending)
            .ToListAsync();
    }

    // Update company
    public async Task UpdateAsync(Company company)
    {
        _context.Companies.Update(company);
        await _context.SaveChangesAsync();
    }

    // Check duplicate email
    public async Task<bool> ExistsByEmailAsync(string companyEmail)
    {
        return await _context.Companies
            .AnyAsync(c => c.CompanyEmail == companyEmail);
    }
}

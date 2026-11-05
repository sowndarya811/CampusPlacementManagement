using CampusPlacementManagement.Data;
using CampusPlacementManagement.Enums;
using CampusPlacementManagement.Interfaces;
using CampusPlacementManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace CampusPlacementManagement.Repositories;

public class CollegeRepository : ICollegeRepository
{
    private readonly ApplicationDbContext _context;

    public CollegeRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<College> AddAsync(College college)
    {
        await _context.Colleges.AddAsync(college);
        await _context.SaveChangesAsync();

        return college;
    }

    public async Task<College?> GetByIdAsync(int id)
    {
        return await _context.Colleges
            .FirstOrDefaultAsync(c => c.CollegeId == id);
    }

    public async Task<List<College>> GetAllAsync()
    {
        return await _context.Colleges
            .AsNoTracking()
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();
    }

    public async Task<List<College>> GetPendingAsync()
    {
        return await _context.Colleges
            .AsNoTracking()
            .Where(c => c.Status == CollegeStatus.Pending)
            .OrderBy(c => c.CreatedAt)
            .ToListAsync();
    }

    public async Task<bool> CodeExistsAsync(string code)
    {
        return await _context.Colleges
            .AnyAsync(c => c.CollegeCode == code);
    }

    public async Task<bool> EmailExistsAsync(string email)
    {
        return await _context.Colleges
            .AnyAsync(c => c.OfficialEmail == email);
    }

    public async Task<bool> CodeExistsForOtherCollegeAsync(
        string code, int id)
    {
        return await _context.Colleges
            .AnyAsync(c => c.CollegeCode == code &&
                           c.CollegeId != id);
    }

    public Task UpdateAsync(College college)
    {
        _context.Colleges.Update(college);
        return Task.CompletedTask;
    }

    public async Task<bool> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync() > 0;
    }
}
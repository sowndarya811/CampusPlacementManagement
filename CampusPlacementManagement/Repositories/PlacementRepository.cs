using CampusPlacementManagement.Data;
using CampusPlacementManagement.Interfaces;
using CampusPlacementManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace CampusPlacementManagement.Repositories;

public class PlacementRepository : IPlacementRepository
{
    private readonly ApplicationDbContext _context;

    public PlacementRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Placement placement)
    {
        await _context.Placements.AddAsync(placement);
        await _context.SaveChangesAsync();
    }

    public async Task<Placement?> GetByIdAsync(int id)
    {
        return await _context.Placements
            .Include(p => p.Company)
            .Include(p => p.College)
            .FirstOrDefaultAsync(p => p.PlacementId == id);
    }

    public async Task<List<Placement>> GetByCollegeIdAsync(int collegeId)
    {
        return await _context.Placements
            .AsNoTracking()
            .Include(p => p.Company)
            .Include(p => p.College)
            .Where(p => p.CollegeId == collegeId)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
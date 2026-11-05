using CampusPlacementManagement.Data;
using CampusPlacementManagement.Interfaces;
using CampusPlacementManagement.Models;
using CampusPlacementManagement.Enums;
using Microsoft.EntityFrameworkCore;

namespace CampusPlacementManagement.Repositories;

public class PlacementOfficerRepository : IPlacementOfficerRepository
{
    private readonly ApplicationDbContext _context;

    public PlacementOfficerRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(PlacementOfficer officer)
    {
        await _context.PlacementOfficers.AddAsync(officer);
        await _context.SaveChangesAsync();
    }

    public async Task<PlacementOfficer?> GetByIdAsync(int id)
    {
        return await _context.PlacementOfficers
            .Include(x => x.User)
            .Include(x => x.College)
            .FirstOrDefaultAsync(x => x.PlacementOfficerId == id);
    }

    public async Task<PlacementOfficer?> GetByUserIdAsync(string userId)
    {
        return await _context.PlacementOfficers
            .Include(x => x.User)
            .Include(x => x.College)
            .FirstOrDefaultAsync(x => x.UserId == userId);
    }

    public async Task<List<PlacementOfficer>> GetAllAsync()
    {
        return await _context.PlacementOfficers
            .Include(x => x.User)
            .Include(x => x.College)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<List<PlacementOfficer>> GetPendingAsync()
    {
        return await _context.PlacementOfficers
            .Include(x => x.User)
            .Include(x => x.College)
            .Where(x => x.Status == PlacementOfficerStatus.Pending)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
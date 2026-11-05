using CampusPlacementManagement.Data;
using CampusPlacementManagement.Interfaces;
using CampusPlacementManagement.Models;
using Microsoft.EntityFrameworkCore;
using CampusPlacementManagement.Enums;

namespace CampusPlacementManagement.Repositories;

public class ApplicationRepository : IApplicationRepository
{
    private readonly ApplicationDbContext _context;

    public ApplicationRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(PlacementApplication application)
    {
        await _context.Applications.AddAsync(application);
        await _context.SaveChangesAsync();
    }

    public async Task<PlacementApplication?> GetByIdAsync(int id)
    {
        return await _context.Applications
            .Include(a => a.Student)
            .Include(a => a.Placement)
                .ThenInclude(p => p.Company)
            .FirstOrDefaultAsync(a => a.ApplicationId == id);
    }

    public async Task<bool> ExistsAsync(int studentId, int placementId)
    {
        return await _context.Applications.AnyAsync(a =>
            a.StudentId == studentId &&
            a.PlacementId == placementId &&
            a.Status != Enums.ApplicationStatus.Withdrawn);
    }

    public async Task<List<PlacementApplication>> GetByStudentIdAsync(
        int studentId)
    {
        return await _context.Applications
            .AsNoTracking()
            .Include(a => a.Student)
            .Include(a => a.Placement)
                .ThenInclude(p => p.Company)
            .Where(a => a.StudentId == studentId)
            .OrderByDescending(a => a.AppliedAt)
            .ToListAsync();
    }

    public async Task<List<PlacementApplication>> GetByPlacementIdAsync(
        int placementId)
    {
        return await _context.Applications
            .AsNoTracking()
            .Include(a => a.Student)
            .Include(a => a.Placement)
                .ThenInclude(p => p.Company)
            .Where(a => a.PlacementId == placementId)
            .OrderByDescending(a => a.AppliedAt)
            .ToListAsync();
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
using CampusPlacementManagement.Data;
using CampusPlacementManagement.Interfaces;
using CampusPlacementManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace CampusPlacementManagement.Repositories;

public class InterviewRepository : IInterviewRepository
{
    private readonly ApplicationDbContext _context;

    public InterviewRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Interview interview)
    {
        await _context.Interviews.AddAsync(interview);
        await _context.SaveChangesAsync();
    }

    public async Task<Interview?> GetByIdAsync(int id)
    {
        return await _context.Interviews
            .Include(i => i.Application)
                .ThenInclude(a => a.Student)
            .Include(i => i.Application)
                .ThenInclude(a => a.Placement)
                    .ThenInclude(p => p.Company)
            .Include(i => i.PlacementOfficer)
            .FirstOrDefaultAsync(i => i.InterviewId == id);
    }

    public async Task<List<Interview>> GetByPlacementOfficerAsync(
        int officerId)
    {
        return await _context.Interviews
            .AsNoTracking()
            .Include(i => i.Application)
                .ThenInclude(a => a.Student)
            .Include(i => i.Application)
                .ThenInclude(a => a.Placement)
                    .ThenInclude(p => p.Company)
            .Where(i => i.PlacementOfficerId == officerId)
            .OrderBy(i => i.InterviewDate)
            .ToListAsync();
    }

    public async Task<List<Interview>> GetByStudentAsync(int studentId)
    {
        return await _context.Interviews
            .AsNoTracking()
            .Include(i => i.Application)
                .ThenInclude(a => a.Student)
            .Include(i => i.Application)
                .ThenInclude(a => a.Placement)
                    .ThenInclude(p => p.Company)
            .Where(i => i.Application.StudentId == studentId)
            .OrderBy(i => i.InterviewDate)
            .ToListAsync();
    }

    public async Task<bool> HasScheduledInterviewAsync(int applicationId)
    {
        return await _context.Interviews.AnyAsync(i =>
            i.ApplicationId == applicationId &&
            i.Status == Enums.InterviewStatus.Scheduled);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
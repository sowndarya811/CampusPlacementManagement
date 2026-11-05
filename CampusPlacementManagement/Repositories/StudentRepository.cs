using CampusPlacementManagement.Data;
using CampusPlacementManagement.Enums;
using CampusPlacementManagement.Interfaces;
using CampusPlacementManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace CampusPlacementManagement.Repositories;

public class StudentRepository : IStudentRepository
{
    private readonly ApplicationDbContext _context;

    public StudentRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Student> AddAsync(Student student)
    {
        await _context.Students.AddAsync(student);
        await _context.SaveChangesAsync();

        return student;
    }

    public async Task<Student?> GetByIdAsync(int id)
    {
        return await _context.Students
            .FirstOrDefaultAsync(s => s.StudentId == id);
    }

    public async Task<Student?> GetByUserIdAsync(string userId)
    {
        return await _context.Students
            .FirstOrDefaultAsync(s => s.UserId == userId);
    }

    public async Task<List<Student>> GetAllAsync()
    {
        return await _context.Students
            .AsNoTracking()
            .OrderByDescending(s => s.CreatedAt)
            .ToListAsync();
    }

    public async Task<List<Student>> GetPendingAsync(int collegeId)
    {
        return await _context.Students
            .AsNoTracking()
            .Where(s =>
                s.CollegeId == collegeId &&
                s.Status == StudentStatus.Pending)
            .OrderBy(s => s.CreatedAt)
            .ToListAsync();
    }

    public async Task<bool> ExistsByRegisterNumberAsync(
        string registerNumber, int collegeId)
    {
        return await _context.Students.AnyAsync(s =>
            s.RegisterNumber == registerNumber &&
            s.CollegeId == collegeId);
    }

    public async Task<bool> ExistsByRegisterNumberForOtherStudentAsync(
        string registerNumber, int collegeId, int studentId)
    {
        return await _context.Students.AnyAsync(s =>
            s.RegisterNumber == registerNumber &&
            s.CollegeId == collegeId &&
            s.StudentId != studentId);
    }

    public Task UpdateAsync(Student student)
    {
        _context.Students.Update(student);
        return Task.CompletedTask;
    }

    public async Task<bool> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync() > 0;
    }
}
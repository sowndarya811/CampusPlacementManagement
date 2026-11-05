using CampusPlacementManagement.Models;

namespace CampusPlacementManagement.Interfaces;

public interface IStudentRepository
{
    Task<Student> AddAsync(Student student);

    Task<Student?> GetByIdAsync(int id);

    Task<Student?> GetByUserIdAsync(string userId);

    Task<List<Student>> GetAllAsync();

    Task<List<Student>> GetPendingAsync(int collegeId);

    Task<bool> ExistsByRegisterNumberAsync(
        string registerNumber, int collegeId);

    Task<bool> ExistsByRegisterNumberForOtherStudentAsync(
        string registerNumber, int collegeId, int studentId);

    Task UpdateAsync(Student student);

    Task<bool> SaveChangesAsync();
}
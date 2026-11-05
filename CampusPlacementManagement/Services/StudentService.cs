using CampusPlacementManagement.DTOs.Student;
using CampusPlacementManagement.Enums;
using CampusPlacementManagement.Interfaces;
using CampusPlacementManagement.Models;
using Microsoft.AspNetCore.Identity;

namespace CampusPlacementManagement.Services;

public class StudentService : IStudentService
{
    private readonly IStudentRepository _studentRepository;
    private readonly UserManager<ApplicationUser> _userManager;

    public StudentService(
        IStudentRepository studentRepository,
        UserManager<ApplicationUser> userManager)
    {
        _studentRepository = studentRepository;
        _userManager = userManager;
    }

    // Enroll a new student
    public async Task<StudentResponseDto> EnrollAsync(StudentCreateDto request)
    {
        var existingUser = await _userManager.FindByEmailAsync(request.Email);

        if (existingUser != null)
            throw new InvalidOperationException("Email already exists.");

        bool registerNumberExists =
            await _studentRepository.ExistsByRegisterNumberAsync(
                request.RegisterNumber,
                request.CollegeId);

        if (registerNumberExists)
            throw new InvalidOperationException(
                "Register number already exists in this college.");

        var user = new ApplicationUser
        {
            UserName = request.Email,
            Email = request.Email,
            FullName = request.FullName,
            IsActive = false
        };

        var userResult = await _userManager.CreateAsync(user, request.Password);

        if (!userResult.Succeeded)
        {
            var errors = string.Join(", ",
                userResult.Errors.Select(e => e.Description));

            throw new ArgumentException(errors);
        }

        try
        {
            var roleResult = await _userManager.AddToRoleAsync(user, "Student");

            if (!roleResult.Succeeded)
            {
                var errors = string.Join(", ",
                    roleResult.Errors.Select(e => e.Description));

                throw new InvalidOperationException(errors);
            }

            var student = new Student
            {
                UserId = user.Id,
                CollegeId = request.CollegeId,

                // Temporary placeholder until PlacementOfficer module
                // provides the authenticated officer's profile ID.
                PlacementOfficerId = 0,

                RegisterNumber = request.RegisterNumber,
                Department = request.Department,
                Degree = request.Degree,
                GraduationYear = request.GraduationYear,
                CGPA = request.CGPA,
                ActiveBacklogs = request.ActiveBacklogs,
                ResumeUrl = request.ResumeUrl,
                Status = StudentStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };

            await _studentRepository.AddAsync(student);

            return await MapToResponseAsync(student);
        }
        catch
        {
            // Remove the Identity account if student creation fails.
            await _userManager.DeleteAsync(user);
            throw;
        }
    }

    // Get all students
    public async Task<List<StudentResponseDto>> GetAllAsync()
    {
        var students = await _studentRepository.GetAllAsync();

        var result = new List<StudentResponseDto>();

        foreach (var student in students)
        {
            result.Add(await MapToResponseAsync(student));
        }

        return result;
    }

    // Get pending students by college
    public async Task<List<StudentResponseDto>> GetPendingAsync(int collegeId)
    {
        var students = await _studentRepository.GetPendingAsync(collegeId);

        var result = new List<StudentResponseDto>();

        foreach (var student in students)
        {
            result.Add(await MapToResponseAsync(student));
        }

        return result;
    }

    // Get student by ID
    public async Task<StudentResponseDto> GetByIdAsync(int id)
    {
        var student = await _studentRepository.GetByIdAsync(id);

        if (student == null)
            throw new KeyNotFoundException("Student not found.");

        return await MapToResponseAsync(student);
    }

    // Approve student
    public async Task<StudentResponseDto> ApproveAsync(int id)
    {
        var student = await _studentRepository.GetByIdAsync(id);

        if (student == null)
            throw new KeyNotFoundException("Student not found.");

        if (student.Status != StudentStatus.Pending)
            throw new InvalidOperationException(
                "Only pending students can be approved.");

        var user = await _userManager.FindByIdAsync(student.UserId);

        if (user == null)
            throw new KeyNotFoundException("Student user account not found.");

        student.Status = StudentStatus.Approved;
        student.ApprovedAt = DateTime.UtcNow;

        user.IsActive = true;

        var userResult = await _userManager.UpdateAsync(user);

        if (!userResult.Succeeded)
            throw new InvalidOperationException(
                "Unable to activate student account.");

        await _studentRepository.UpdateAsync(student);
        await _studentRepository.SaveChangesAsync();

        return await MapToResponseAsync(student);
    }

    // Reject student
    public async Task<StudentResponseDto> RejectAsync(int id)
    {
        var student = await _studentRepository.GetByIdAsync(id);

        if (student == null)
            throw new KeyNotFoundException("Student not found.");

        if (student.Status != StudentStatus.Pending)
            throw new InvalidOperationException(
                "Only pending students can be rejected.");

        student.Status = StudentStatus.Rejected;

        await _studentRepository.UpdateAsync(student);
        await _studentRepository.SaveChangesAsync();

        return await MapToResponseAsync(student);
    }

    // Suspend student
    public async Task<StudentResponseDto> SuspendAsync(int id)
    {
        var student = await _studentRepository.GetByIdAsync(id);

        if (student == null)
            throw new KeyNotFoundException("Student not found.");

        var user = await _userManager.FindByIdAsync(student.UserId);

        if (user == null)
            throw new KeyNotFoundException("Student user account not found.");

        student.Status = StudentStatus.Suspended;
        user.IsActive = false;

        var userResult = await _userManager.UpdateAsync(user);

        if (!userResult.Succeeded)
            throw new InvalidOperationException(
                "Unable to deactivate student account.");

        await _studentRepository.UpdateAsync(student);
        await _studentRepository.SaveChangesAsync();

        return await MapToResponseAsync(student);
    }

    // Update student details
    public async Task<StudentResponseDto> UpdateAsync(
        int id,
        StudentUpdateDto request)
    {
        var student = await _studentRepository.GetByIdAsync(id);

        if (student == null)
            throw new KeyNotFoundException("Student not found.");

        bool duplicateRegisterNumber =
            await _studentRepository.ExistsByRegisterNumberForOtherStudentAsync(
                request.RegisterNumber,
                student.CollegeId,
                id);

        if (duplicateRegisterNumber)
            throw new InvalidOperationException(
                "Register number already exists.");

        var user = await _userManager.FindByIdAsync(student.UserId);

        if (user == null)
            throw new KeyNotFoundException("Student user account not found.");

        user.FullName = request.FullName;

        var userResult = await _userManager.UpdateAsync(user);

        if (!userResult.Succeeded)
            throw new InvalidOperationException(
                "Unable to update student user details.");

        student.RegisterNumber = request.RegisterNumber;
        student.Department = request.Department;
        student.Degree = request.Degree;
        student.GraduationYear = request.GraduationYear;
        student.CGPA = request.CGPA;
        student.ActiveBacklogs = request.ActiveBacklogs;
        student.ResumeUrl = request.ResumeUrl;

        await _studentRepository.UpdateAsync(student);
        await _studentRepository.SaveChangesAsync();

        return await MapToResponseAsync(student);
    }

    // Get logged-in student's profile
    public async Task<StudentResponseDto> GetByUserIdAsync(string userId)
    {
        var student = await _studentRepository.GetByUserIdAsync(userId);

        if (student == null)
            throw new KeyNotFoundException("Student profile not found.");

        return await MapToResponseAsync(student);
    }

    // Map Student entity to response DTO
    private async Task<StudentResponseDto> MapToResponseAsync(Student student)
    {
        var user = await _userManager.FindByIdAsync(student.UserId);

        return new StudentResponseDto
        {
            StudentId = student.StudentId,
            UserId = student.UserId,
            FullName = user?.FullName ?? string.Empty,
            Email = user?.Email ?? string.Empty,
            CollegeId = student.CollegeId,
            PlacementOfficerId = student.PlacementOfficerId,
            RegisterNumber = student.RegisterNumber,
            Department = student.Department,
            Degree = student.Degree,
            GraduationYear = student.GraduationYear,
            CGPA = student.CGPA,
            ActiveBacklogs = student.ActiveBacklogs,
            ResumeUrl = student.ResumeUrl,
            Status = student.Status,
            ApprovedAt = student.ApprovedAt,
            CreatedAt = student.CreatedAt
        };
    }
}
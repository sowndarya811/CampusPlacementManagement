
using CampusPlacementManagement.Models;
using CampusPlacementManagement.Enums;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CampusPlacementManagement.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // DbSets
    public DbSet<Company> Companies => Set<Company>();

    public DbSet<Student> Students => Set<Student>();

    public DbSet<College> Colleges => Set<College>();

    public DbSet<PlacementOfficer> PlacementOfficers => Set<PlacementOfficer>();

    public DbSet<Placement> Placements => Set<Placement>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Company Configuration
        modelBuilder.Entity<Company>(entity =>
        {
            entity.HasKey(c => c.CompanyId);

            entity.Property(c => c.CompanyName)
                .IsRequired()
                .HasMaxLength(150);

            entity.Property(c => c.CompanyEmail)
                .IsRequired()
                .HasMaxLength(150);

            entity.HasIndex(c => c.CompanyEmail)
                .IsUnique();
        });

        // Student Configuration
        modelBuilder.Entity<Student>(entity =>
        {
            entity.HasKey(s => s.StudentId);

            entity.Property(s => s.UserId)
                .IsRequired();

            entity.Property(s => s.RegisterNumber)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(s => s.Department)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(s => s.Degree)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(s => s.CGPA)
                .HasPrecision(4, 2);

            entity.Property(s => s.ResumeUrl)
                .HasMaxLength(500);

            entity.Property(s => s.Status)
                .HasConversion<int>();

            entity.HasIndex(s => new
            {
                s.RegisterNumber,
                s.CollegeId
            }).IsUnique();

            entity.HasIndex(s => s.UserId)
                .IsUnique();

            entity.HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(s => s.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // College Configuration
        modelBuilder.Entity<College>(entity =>
        {
            entity.HasKey(c => c.CollegeId);

            entity.Property(c => c.CollegeName)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(c => c.CollegeCode)
                .IsRequired()
                .HasMaxLength(50);

            entity.HasIndex(c => c.CollegeCode)
                .IsUnique();

            entity.Property(c => c.UniversityName)
                .HasMaxLength(200);

            entity.Property(c => c.OfficialEmail)
                .IsRequired()
                .HasMaxLength(255);

            entity.Property(c => c.PhoneNumber)
                .IsRequired()
                .HasMaxLength(20);

            entity.Property(c => c.Address)
                .IsRequired()
                .HasMaxLength(500);

            entity.Property(c => c.Website)
                .HasMaxLength(255);

            entity.Property(c => c.Status)
                .HasConversion<int>();

            entity.Property(c => c.CreatedAt)
                .IsRequired();
        });

        // Placement Officer Configuration
        modelBuilder.Entity<PlacementOfficer>(entity =>
        {
            entity.HasKey(p => p.PlacementOfficerId);

            entity.Property(p => p.UserId)
                .IsRequired();

            entity.HasIndex(p => p.UserId)
                .IsUnique();

            entity.Property(p => p.Designation)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(p => p.Status)
                .HasConversion<int>()
                .IsRequired();

            entity.Property(p => p.ApprovedBy)
                .IsRequired(false);

            entity.Property(p => p.ApprovedAt)
                .IsRequired(false);

            entity.HasOne(p => p.User)
                .WithOne()
                .HasForeignKey<PlacementOfficer>(p => p.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(p => p.College)
                .WithMany()
                .HasForeignKey(p => p.CollegeId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(p => p.CollegeId);
        });

        // Placement Configuration
        modelBuilder.Entity<Placement>(entity =>
        {
            entity.HasKey(p => p.PlacementId);

            entity.Property(p => p.JobTitle)
                .IsRequired()
                .HasMaxLength(150);

            entity.Property(p => p.JobDescription)
                .IsRequired();

            entity.Property(p => p.JobLocation)
                .IsRequired()
                .HasMaxLength(150);

            entity.Property(p => p.EmploymentType)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(p => p.SalaryPackage)
                .HasPrecision(10, 2);

            entity.Property(p => p.MinimumCGPA)
                .HasPrecision(4, 2);

            entity.Property(p => p.EligibleDepartments)
                .IsRequired();

            entity.Property(p => p.Status)
                .HasConversion<int>()
                .IsRequired();

            entity.Property(p => p.CreatedAt)
                .IsRequired();

            // Placement - Company
            entity.HasOne(p => p.Company)
                .WithMany()
                .HasForeignKey(p => p.CompanyId)
                .OnDelete(DeleteBehavior.Restrict);

            // Placement - College
            entity.HasOne(p => p.College)
                .WithMany()
                .HasForeignKey(p => p.CollegeId)
                .OnDelete(DeleteBehavior.Restrict);

            // Placement - Placement Officer
            entity.HasOne(p => p.PlacementOfficer)
                .WithMany()
                .HasForeignKey(p => p.PlacementOfficerId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(p => p.CollegeId);

            entity.HasIndex(p => p.CompanyId);

            entity.HasIndex(p => p.ApplicationDeadline);
        });
    }
}

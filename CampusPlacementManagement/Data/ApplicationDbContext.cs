
using CampusPlacementManagement.Models;
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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // ASP.NET Core Identity configuration
        base.OnModelCreating(modelBuilder);

        // Company table configuration
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

        // Student table configuration
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

            // Unique register number within a college
            entity.HasIndex(s => new
            {
                s.RegisterNumber,
                s.CollegeId
            }).IsUnique();

            // One Identity user per student profile
            entity.HasIndex(s => s.UserId)
                .IsUnique();

            // Student - ApplicationUser relationship
            entity.HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(s => s.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // College table configuration
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
    }
}


using CampusPlacementManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace CampusPlacementManagement.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Company> Companies => Set<Company>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Company Table Configuration
        modelBuilder.Entity<Company>(entity =>
        {
            entity.ToTable("Companies");

            entity.HasKey(c => c.CompanyId);

            entity.Property(c => c.CompanyName)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(c => c.CompanyEmail)
                .IsRequired()
                .HasMaxLength(255);

            entity.HasIndex(c => c.CompanyEmail)
                .IsUnique();

            entity.Property(c => c.Website)
                .HasMaxLength(255);

            entity.Property(c => c.Industry)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(c => c.Address)
                .IsRequired()
                .HasMaxLength(500);

            entity.Property(c => c.Description);

            entity.Property(c => c.Status)
                .IsRequired();

            entity.Property(c => c.CreatedAt)
                .IsRequired();
        });
    }
}

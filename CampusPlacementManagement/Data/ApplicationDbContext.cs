
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

    public DbSet<Company> Companies => Set<Company>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configure ASP.NET Core Identity tables
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

            // Retain your other existing Company
            // Fluent API configurations here.
        });
    }
}

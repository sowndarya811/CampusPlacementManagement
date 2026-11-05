
using CampusPlacementManagement.Data;
using CampusPlacementManagement.Interfaces;
using CampusPlacementManagement.Middleware;
using CampusPlacementManagement.Repositories;
using CampusPlacementManagement.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ======================================
// 1. Add Controllers
// ======================================

builder.Services.AddControllers();

// ======================================
// 2. Configure OpenAPI
// ======================================

builder.Services.AddOpenApi();

// ======================================
// 3. Database Connection
// ======================================

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

// ======================================
// 4. Global Exception Handling
// ======================================

builder.Services.AddProblemDetails();

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

// ======================================
// 5. Repository Dependency Injection
// ======================================

builder.Services.AddScoped<ICompanyRepository, CompanyRepository>();

// ======================================
// 6. Service Dependency Injection
// ======================================

builder.Services.AddScoped<ICompanyService, CompanyService>();

var app = builder.Build();

// ======================================
// 7. Configure HTTP Pipeline
// ======================================

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// ======================================
// 8. Map Controllers
// ======================================

app.MapControllers();

// ======================================
// 9. Run Application
// ======================================

app.Run();

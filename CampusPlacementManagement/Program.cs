
using System.Text;
using CampusPlacementManagement.Data;
using CampusPlacementManagement.Interfaces;
using CampusPlacementManagement.Middleware;
using CampusPlacementManagement.Models;
using CampusPlacementManagement.Repositories;
using CampusPlacementManagement.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// 1. Load User Secrets
builder.Configuration.AddUserSecrets<Program>(optional: true);

// 2. Controllers and OpenAPI
builder.Services.AddControllers();
builder.Services.AddOpenApi();

// 3. Database Connection
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlOptions => sqlOptions.CommandTimeout(180)));

// 4. ASP.NET Core Identity Configuration
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequiredLength = 8;
    options.Password.RequireDigit = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = false;

    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// 5. JWT Configuration
var jwtKey = builder.Configuration["Jwt:Key"]
    ?? throw new InvalidOperationException("JWT Key is missing.");

var jwtIssuer = builder.Configuration["Jwt:Issuer"]
    ?? throw new InvalidOperationException("JWT Issuer is missing.");

var jwtAudience = builder.Configuration["Jwt:Audience"]
    ?? throw new InvalidOperationException("JWT Audience is missing.");

if (Encoding.UTF8.GetByteCount(jwtKey) < 32)
{
    throw new InvalidOperationException(
        "JWT Key must contain at least 32 bytes.");
}

// 6. Authentication Configuration
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme =
        JwtBearerDefaults.AuthenticationScheme;

    options.DefaultChallengeScheme =
        JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,

        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,

        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtKey)),

        ClockSkew = TimeSpan.Zero
    };
});

// 7. Authorization
builder.Services.AddAuthorization();

// 8. Exception Handling
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

// 9. Company Management Dependency Injection
builder.Services.AddScoped<ICompanyRepository, CompanyRepository>();
builder.Services.AddScoped<ICompanyService, CompanyService>();

// 10. Student Management Dependency Injection
builder.Services.AddScoped<IStudentRepository, StudentRepository>();
builder.Services.AddScoped<IStudentService, StudentService>();

// 11. Authentication Dependency Injection
builder.Services.AddScoped<IAuthService, AuthService>();

// 12. Role Management Dependency Injection
builder.Services.AddScoped<IRoleService, RoleService>();

// 13. College Management Dependency Injection
builder.Services.AddScoped<ICollegeRepository, CollegeRepository>();
builder.Services.AddScoped<ICollegeService, CollegeService>();

// 14. Placement Officer Management Dependency Injection
builder.Services.AddScoped<
    IPlacementOfficerRepository,
    PlacementOfficerRepository>();

builder.Services.AddScoped<
    IPlacementOfficerService,
    PlacementOfficerService>();

// 15. Placement Management Dependency Injection
builder.Services.AddScoped<IPlacementRepository, PlacementRepository>();
builder.Services.AddScoped<IPlacementService, PlacementService>();

// 16. Application Management Dependency Injection
builder.Services.AddScoped<IApplicationRepository, ApplicationRepository>();
builder.Services.AddScoped<IApplicationService, ApplicationService>();

// 17. Interview and Selection Management Dependency Injection
builder.Services.AddScoped<IInterviewRepository, InterviewRepository>();
builder.Services.AddScoped<IInterviewService, InterviewService>();

// 18. Notification Management Dependency Injection
builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
builder.Services.AddScoped<INotificationService, NotificationService>();

// 19. Reports and Dashboard Management Dependency Injection
builder.Services.AddScoped<IDashboardService, DashboardService>();

var app = builder.Build();

// Admin Configuration Check
Console.WriteLine(
    $"Admin Email Configured: {!string.IsNullOrWhiteSpace(app.Configuration["BootstrapAdmin:Email"])}");

Console.WriteLine(
    $"Admin Password Configured: {!string.IsNullOrWhiteSpace(app.Configuration["BootstrapAdmin:Password"])}");

Console.WriteLine(
    $"Admin Name Configured: {!string.IsNullOrWhiteSpace(app.Configuration["BootstrapAdmin:FullName"])}");

// 20. Seed Default Roles and Initial SuperAdmin
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    await IdentitySeeder.SeedRolesAsync(services);
}

// 21. Middleware Pipeline
app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();

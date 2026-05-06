using System.Data;
using System.Text;
using DotNetEnv;
using EExamSystem.Api.Configuration;
using EExamSystem.Api.Data;
using EExamSystem.Shared.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using Microsoft.IdentityModel.Tokens;
using EExamSystem.Api.Interfaces;
using EExamSystem.Api.Services;
using EExamSystem.Shared.DTOs;
using Microsoft.AspNetCore.Mvc;

Env.Load();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
{
    var connString = Environment.GetEnvironmentVariable("CONNECTION_STRING");
    options.UseNpgsql(connString);
});


// Add services to the container.

builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.InvalidModelStateResponseFactory = context =>
        {
            var errors = context.ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();

            // Wrap them in your standard ServiceResponse
            var response = new ServiceResponse<object>
            {
                Success = false,
                Message = "Validation failed. Please check the provided data.",
                Data = new { Errors = errors },
                StatusCode = StatusCodes.Status400BadRequest
            };

            return new BadRequestObjectResult(response);
        };
    });

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi(options =>
{
    options.AddOperationTransformer<SecurityRequirementsTransformer>();
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazorClient", policy =>
    {
        //TODO: Change later to frontend url
        policy.AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// Configure identity services
builder.Services.AddIdentity<User, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 8;

    // Signin manager options
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

// Get JWT configuration from appsettings file
var jwtSettings = builder.Configuration.GetSection("JWT");
var jwtSecretKey = Environment.GetEnvironmentVariable("JWT_SECRET_KEY");
var issuer = jwtSettings["Issuer"];
var audience = jwtSettings["Audience"];
var durationString = jwtSettings["ExpirationInMinutes"];

// Check if jwt configuration is not configured
if (string.IsNullOrWhiteSpace(jwtSecretKey))
{
    throw new InvalidOperationException(
        "JWT Secret Key is insecure or missing.");
}

if (string.IsNullOrEmpty(issuer) || string.IsNullOrEmpty(audience))
{
    throw new InvalidOperationException("JWT Issuer or Audience is not configured.");
}

if (!double.TryParse(durationString, out double durationInDays))
{
    throw new InvalidOperationException(
        $"JWT ExpirationInMinutes '{durationString}' is not a valid number.");
}

var key = Encoding.UTF8.GetBytes(jwtSecretKey);

// Configure JWT authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key),

        RoleClaimType = System.Security.Claims.ClaimTypes.Role
    };
});

builder.Services.AddAuthorization();

// Register services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ITestbankService, TestbankService>();

// Register user service
builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddScoped<ISectionService, SectionService>();
builder.Services.AddScoped<IEnrollmentService, EnrollmentService>();
builder.Services.AddScoped<IExamAssignmentService, ExamAssignmentService>();
builder.Services.AddScoped<IStudentExamsService, StudentExamsService>();


// Ensure roles are created in the database
var serviceProvider = builder.Services.BuildServiceProvider();
using var scope = serviceProvider.CreateScope();
var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
string[] roles = { UserRole.Admin, UserRole.Chair, UserRole.Instructor, UserRole.Student };

foreach (var role in roles)
{
    if (!await roleManager.RoleExistsAsync(role))
    {
        await roleManager.CreateAsync(new IdentityRole(role));
    }
}

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.UseCors("AllowBlazorClient");

// To identify user
app.UseAuthentication();

// To check user's permissions
app.UseAuthorization();

app.MapControllers();

app.Run();
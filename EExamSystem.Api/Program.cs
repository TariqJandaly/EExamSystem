using System.Data;
using System.Text;
using DotNetEnv;
using EExamSystem.Api.Data;
using EExamSystem.Shared.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using Microsoft.IdentityModel.Tokens;
using EExamSystem.Api.Interfaces;

Env.Load();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
{
    var connString = Environment.GetEnvironmentVariable("CONNECTION_STRING");
    options.UseNpgsql(connString);
});

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

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
var expirationInMinutes = jwtSettings["ExpirationInMinutes"];

// Check if Secret Key is not configured
if (string.IsNullOrEmpty(jwtSecretKey) ||
    string.IsNullOrEmpty(issuer) ||
    string.IsNullOrEmpty(audience) ||
    string.IsNullOrEmpty(expirationInMinutes))
{
    throw new InvalidOperationException("JWT configuration is not complete in Environment Variables.");
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
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});

builder.Services.AddAuthorization();

// Register authentication service
builder.Services.AddScoped<IAuthService, AuthService>();

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
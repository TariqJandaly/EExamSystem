using EExamSystem.Api.Interfaces;
using EExamSystem.Shared.DTOs.Auth;
using EExamSystem.Shared.Models;

using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

public class AuthService : IAuthService
{
    private readonly UserManager<User> _userManager;
    private readonly IConfiguration _configuration;

    public AuthService(UserManager<User> userManager, IConfiguration configuration)
    {
        _userManager = userManager;
        _configuration = configuration;
    }

    private (string Token, DateTime Expiry) GenerateJwtToken(User user)
    {
        // Prepare User informations that will be into the token
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Email, user.Email!),
            new Claim("FullName", user.FullName ?? ""),

            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()) // Unique id for token
        };

        // Token Settings
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expiry = DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["Jwt:DurationInMinutes"]));

        // Generate Token
        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: expiry,
            signingCredentials: creds
        );

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
        return (Token: tokenString, Expiry: expiry);
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto)
    {
        // Create new user object
        var user = new User
        {
            Email = registerDto.Email,
            FullName = registerDto.FullName,
            UserName = registerDto.Email // UserName is required
        };

        // Try to create user
        var result = await _userManager.CreateAsync(user, registerDto.Password);

        // If creating process is failed return the errors
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            return new AuthResponseDto
            {
                IsSuccess = false,
                Message = errors
            };
        }

        // Generate token and return
        var generatedToken = GenerateJwtToken(user);
        return new AuthResponseDto
        {
            IsSuccess = true,
            Token = generatedToken.Token,
            Expiry = generatedToken.Expiry,
            Message = "RegisterSuccess"
        };
    }

    public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto)
    {
        // Search for User with email
        var user = await _userManager.FindByEmailAsync(loginDto.Email);

        // Check if user not exists or password is incorrect
        if (user == null || !await _userManager.CheckPasswordAsync(user, loginDto.Password))
        {
            return new AuthResponseDto
            {
                IsSuccess = false,
                Message = "EmailOrPasswordIncorrect"
            };
        }

        // Generate token and return
        var generatedToken = GenerateJwtToken(user);
        return new AuthResponseDto
        {
            IsSuccess = true,
            Token = generatedToken.Token,
            Expiry = generatedToken.Expiry,
            Message = "LoginSuccess"
        };

    }
}
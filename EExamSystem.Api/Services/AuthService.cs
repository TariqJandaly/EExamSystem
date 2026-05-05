using EExamSystem.Api.Interfaces;
using EExamSystem.Shared.DTOs.Auth;
using EExamSystem.Shared.Models;

using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Superpower.Model;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

public class AuthService : IAuthService
{
    private readonly UserManager<User> _userManager;
    private readonly IConfiguration _configuration;
    private readonly SignInManager<User> _signInManager;

    public AuthService(UserManager<User> userManager, IConfiguration configuration, SignInManager<User> signInManager)
    {
        _userManager = userManager;
        _configuration = configuration;
        _signInManager = signInManager;
    }

    /// <summary>
    /// Generates a JWT token for the authenticated user, including claims such as user ID, email, and full name. The token is signed using a secret key and has an expiration time based on the configuration settings.
    /// </summary>
    /// <param name="user">The user for whom to generate the token.</param>
    /// <returns>A tuple containing the generated token and its expiration time.</returns>
    private async Task<(string Token, DateTime Expiry)> GenerateJwtToken(User user)
    {
        // get user roles
        var userRoles = await _userManager.GetRolesAsync(user);

        // Prepare User informations that will be into the token
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Email, user.Email!),
            new Claim("FullName", user.FullName ?? ""),

            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()) // Unique id for token
        };

        foreach (var role in userRoles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        // Token Settings
        var jwtSecretKey = Environment.GetEnvironmentVariable("JWT_SECRET_KEY");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecretKey!));
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

    /// <summary>
    /// Registers a new user with the provided registration details.
    /// </summary>
    /// <param name="registerDto">The registration details for the new user.</param>
    /// <returns>A task representing the asynchronous operation, returning the authentication response.</returns>
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

        // add default role
        await _userManager.AddToRoleAsync(user, "User");

        // Generate token and return
        var generatedToken = await GenerateJwtToken(user);
        return new AuthResponseDto
        {
            IsSuccess = true,
            Token = generatedToken.Token,
            Expiry = generatedToken.Expiry,
            Message = "RegisterSuccess"
        };
    }

    /// <summary>
    /// Authenticates a user with the provided login credentials.
    /// </summary>
    /// <param name="loginDto">The login details for the user.</param>
    /// <returns>A task representing the asynchronous operation, returning the authentication response.</returns>
    public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto)
    {
        // Search for User with email
        var user = await _userManager.FindByEmailAsync(loginDto.Email);

        if (user == null)
        {
            return new AuthResponseDto
            {
                IsSuccess = false,
                Message = "EmailOrPasswordIncorrect"
            };
        }

        var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, true);

        if (result.IsLockedOut)
        {
            return new AuthResponseDto
            {
                IsSuccess = false,
                Message = "UserLockedOut"
            };
        }

        // Check if user not exists or password is incorrect
        if (!result.Succeeded)
        {
            return new AuthResponseDto
            {
                IsSuccess = false,
                Message = "EmailOrPasswordIncorrect"
            };
        }

        // Generate token and return
        var generatedToken = await GenerateJwtToken(user);
        return new AuthResponseDto
        {
            IsSuccess = true,
            Token = generatedToken.Token,
            Expiry = generatedToken.Expiry,
            Message = "LoginSuccess"
        };

    }
}
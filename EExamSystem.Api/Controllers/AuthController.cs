using EExamSystem.Api.Interfaces;
using EExamSystem.Shared.DTOs.Auth;
using Microsoft.AspNetCore.Mvc;

namespace EExamSystem.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
    {
        var result = await _authService.RegisterAsync(registerDto);

        if (result.IsSuccess)
            return Ok(result); // Status 200 with auth data

        return BadRequest(result.Message); // Status 400
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
        var result = await _authService.LoginAsync(loginDto);

        if (result.IsSuccess)
            return Ok(result); // Status 200 with auth data

        return Unauthorized(result.Message); // Status 401
    }
}
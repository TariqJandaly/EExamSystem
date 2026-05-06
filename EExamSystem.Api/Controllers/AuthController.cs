using EExamSystem.Api.Interfaces;
using EExamSystem.Shared.DTOs;
using EExamSystem.Shared.DTOs.Auth;
using Microsoft.AspNetCore.Mvc;

namespace EExamSystem.Api.Controllers;

/// <summary>
/// Handles user authentication, including registration and login.
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>
    /// Registers a new user account.
    /// </summary>
    /// <param name="registerDto">The registration payload. See <see cref="RegisterDto"/>.</param>
    /// <returns>A <see cref="ServiceResponse"/> confirming successful registration.</returns>
    /// <response code="200">Registration succeeded.</response>
    /// <response code="400">
    /// Registration failed. Possible <c>Message</c> values:
    /// <list type="bullet">
    ///   <item><c>EmailAlreadyExists</c> – The provided email is already registered.</item>
    ///   <item><c>PasswordTooWeak</c> – The password does not meet complexity requirements.</item>
    /// </list>
    /// </response>
    [HttpPost("register")]
    [ProducesResponseType(typeof(ServiceResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorServiceResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
    {
        var result = await _authService.RegisterAsync(registerDto);

        if (result.IsSuccess)
            return Ok(result);

        return BadRequest(new ErrorServiceResponse { Success = false, Message = result.Message, StatusCode = 400 });
    }

    /// <summary>
    /// Authenticates a user and returns a JWT bearer token.
    /// </summary>
    /// <param name="loginDto">The login credentials. See <see cref="LoginDto"/>.</param>
    /// <returns>A <see cref="ServiceResponse"/> containing the JWT token on success.</returns>
    /// <response code="200">Authentication succeeded. The response body contains the JWT token.</response>
    /// <response code="401">
    /// Authentication failed. Possible <c>Message</c> values:
    /// <list type="bullet">
    ///   <item><c>InvalidCredentials</c> – The email or password is incorrect.</item>
    /// </list>
    /// </response>
    [HttpPost("login")]
    [ProducesResponseType(typeof(ServiceResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorServiceResponse), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
        var result = await _authService.LoginAsync(loginDto);

        if (result.IsSuccess)
            return Ok(result);

        return Unauthorized(new ErrorServiceResponse { Success = false, Message = result.Message, StatusCode = 401 });
    }
}
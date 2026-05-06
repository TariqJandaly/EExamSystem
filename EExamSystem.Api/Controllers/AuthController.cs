using EExamSystem.Api.Interfaces;
using EExamSystem.Shared.DTOs;
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

    /// <summary>
    /// Registers a new user with the provided registration details.
    /// </summary>
    /// <param name="registerDto">
    /// The DTO containing the registration details.
    /// </param>
    /// <returns>Auth data if successful, error message otherwise.</returns>
    /// <response code="200">Returns the authentication data if registration is successful.</response>
    /// <response code="400">Returns an error message.</response>
    [HttpPost("register")]
    [ProducesResponseType(typeof(ServiceResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorServiceResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
    {
        var result = await _authService.RegisterAsync(registerDto);

        if (result.IsSuccess)
            return Ok(result); // Status 200 with auth data

        return BadRequest(new ErrorServiceResponse { Success = false, Message = result.Message, StatusCode = 400 });
    }

    /// <summary>
    /// Authenticates a user with the provided login credentials and returns an authentication token if successful.
    /// </summary>
    /// <param name="loginDto">
    /// The DTO containing the login credentials.
    /// </param>
    /// <returns>
    /// The authentication response containing the token if successful, or an error message otherwise.
    /// </returns>
    /// <response code="200">Returns the authentication data if login is successful.</response>
    /// <response code="401">Returns an error message if authentication fails.</response>
    [HttpPost("login")]
    [ProducesResponseType(typeof(ServiceResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorServiceResponse), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
        var result = await _authService.LoginAsync(loginDto);

        if (result.IsSuccess)
            return Ok(result); // Status 200 with auth data

        return Unauthorized(new ErrorServiceResponse { Success = false, Message = result.Message, StatusCode = 401 });
    }
}
using EExamSystem.Api.Interfaces;
using EExamSystem.Shared.DTOs.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EExamSystem.Shared.DTOs;

namespace EExamSystem.Api.Controllers;

/// <summary>
/// Controller for managing users.
/// </summary>
[Authorize]
[ApiController]
[Route("api/v1/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    /// <summary>
    /// Retrieves a list of all users. Accessible to users with Admin role.
    /// </summary>
    /// <returns>A service response containing the list of users or an error message.</returns>
    /// <response code="200">Returns the list of users if the request is successful.</response>
    /// <response code="400">Returns an error message if the request fails.</response>
    [HttpGet]
    [Authorize(Roles = "CHAIR,ADMIN")]
    [ProducesResponseType(typeof(ServiceResponse<List<UserDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorServiceResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAllUsersAsync()
    {
        var result = await _userService.GetAllUsersAsync();
        return StatusCode(result.StatusCode, result);
    }

    /// <summary>
    /// Retrieves a user by their ID. Accessible to users with Admin role.
    /// </summary>
    /// <param name="id">The ID of the user to retrieve.</param>
    /// <returns>A service response containing the user or an error message.</returns>
    /// <response code="200">Returns the user if the request is successful.</response>
    /// <response code="404">Returns an error message if the user is not found.</response>
    [HttpGet("{id}")]
    [Authorize(Roles = "CHAIR,ADMIN")]
    [ProducesResponseType(typeof(ServiceResponse<UserDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorServiceResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetUserAsync(string id)
    {
        var result = await _userService.GetUserAsync(id);
        return StatusCode(result.StatusCode, result);
    }

    /// <summary>
    /// Creates a new user with the provided details. Accessible to users with Admin role.
    /// </summary>
    /// <param name="userCreateDto">The DTO containing the details of the user to create.</param>
    /// <returns>A service response containing the created user or an error message.</returns>
    /// <response code="200">Returns the created user if the request is successful.</response>
    /// <response code="400">Returns an error message if the request fails.</response>
    [HttpPost]
    [Authorize(Roles = "CHAIR,ADMIN")]
    [ProducesResponseType(typeof(ServiceResponse<UserDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorServiceResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateUserAsync([FromBody] UserCreateDto userCreateDto)
    {
        var result = await _userService.CreateUserAsync(userCreateDto);
        return StatusCode(result.StatusCode, result);
    }

    /// <summary>
    /// Deletes a user by their ID. Accessible to users with Admin role.
    /// </summary>
    /// <param name="id">The ID of the user to delete.</param>
    /// <returns>A service response containing the deleted user or an error message.</returns>
    /// <response code="200">Returns the deleted user if the request is successful.</response>
    /// <response code="404">Returns an error message if the user is not found.</response>
    [HttpDelete("{id}")]
    [Authorize(Roles = "CHAIR,ADMIN")]
    [ProducesResponseType(typeof(ServiceResponse<UserDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorServiceResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteUserAsync(string id)
    {
        var result = await _userService.DeleteUserAsync(id);
        return StatusCode(result.StatusCode, result);
    }

    /// <summary>
    /// Updates a user by their ID with the provided details. Accessible to users with Admin role.
    /// </summary>
    /// <param name="id">The ID of the user to update.</param>
    /// <param name="userCreateDto">The DTO containing the updated details of the user.</param>
    /// <returns>A service response containing the updated user or an error message.</returns>
    /// <response code="200">Returns the updated user if the request is successful.</response>
    /// <response code="404">Returns an error message if the user is not found.</response>
    [HttpPut("{id}")]
    [Authorize(Roles = "CHAIR,ADMIN")]
    [ProducesResponseType(typeof(ServiceResponse<UserDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorServiceResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateUserAsync(string id, [FromBody] UserCreateDto userCreateDto)
    {
        var result = await _userService.UpdateUserAsync(id, userCreateDto);
        return StatusCode(result.StatusCode, result);
    }

    /// <summary>
    /// Adds roles to a user by their ID. Accessible to users with Admin role.
    /// </summary>
    /// <param name="id">The ID of the user to add roles to.</param>
    /// <param name="userRolesDto">The DTO containing the roles to add to the user.</param>
    /// <returns>A service response containing the updated user or an error message.</returns>
    /// <response code="200">Returns the updated user if the request is successful.</response>
    /// <response code="404">Returns an error message if the user is not found.</response>
    [HttpPost("{id}/roles")]
    [Authorize(Roles = "CHAIR,ADMIN")]
    [ProducesResponseType(typeof(ServiceResponse<UserDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorServiceResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddRolesToUserAsync(string id, [FromBody] UserRolesDto userRolesDto)
    {
        var result = await _userService.AddRolesToUserAsync(id, userRolesDto);
        return StatusCode(result.StatusCode, result);
    }

    /// <summary>
    /// Removes roles from a user by their ID. Accessible to users with Admin role.
    /// </summary>
    /// <param name="id">The ID of the user to remove roles from.</param>
    /// <param name="userRolesDto">The DTO containing the roles to remove from the user.</param>
    /// <returns>A service response containing the updated user or an error message.</returns>
    /// <response code="200">Returns the updated user if the request is successful.</response>
    /// <response code="404">Returns an error message if the user is not found.</response>
    [HttpDelete("{id}/roles")]
    [Authorize(Roles = "CHAIR,ADMIN")]
    [ProducesResponseType(typeof(ServiceResponse<UserDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorServiceResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoveRolesFromUserAsync(string id, [FromBody] UserRolesDto userRolesDto)
    {
        var result = await _userService.RemoveRolesFromUserAsync(id, userRolesDto);
        return StatusCode(result.StatusCode, result);
    }

    /// <summary>
    /// Changes the password of a user by their ID. Accessible to users with Admin role.
    /// </summary>
    /// <param name="id">The ID of the user to change the password for.</param>
    /// <param name="changePasswordDto">The DTO containing the new password details.</param>
    /// <returns>A service response indicating the success or failure of the operation.</returns>
    /// <response code="200">Returns a success message if the password change is successful.</response>
    /// <response code="404">Returns an error message if the user is not found.</response>
    [HttpPost("{id}/change-password")]
    [Authorize(Roles = "CHAIR,ADMIN")]
    [ProducesResponseType(typeof(ServiceResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorServiceResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ChangeUserPasswordAsync(string id, [FromBody] UserChangePasswordDto changePasswordDto)
    {
        var result = await _userService.ChangeUserPasswordAsync(id, changePasswordDto);
        return StatusCode(result.StatusCode, result);
    }



}
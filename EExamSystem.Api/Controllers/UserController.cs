using EExamSystem.Api.Interfaces;
using EExamSystem.Shared.DTOs;
using EExamSystem.Shared.DTOs.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EExamSystem.Api.Controllers;

/// <summary>
/// Manages user accounts, role assignments, and credential changes. All routes are restricted to Chair and Admin roles.
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
    /// Retrieves a list of all user accounts in the system.
    /// </summary>
    /// <returns>A <see cref="ServiceResponse{T}"/> containing a list of <see cref="UserDto"/>.</returns>
    /// <response code="200">Successfully retrieved the full user list.</response>
    /// <response code="400">
    /// The request could not be processed. Possible <c>Message</c> values:
    /// <list type="bullet">
    ///   <item><c>FetchFailed</c> – An unexpected error occurred while retrieving users.</item>
    /// </list>
    /// </response>
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
    /// Retrieves a specific user account by their ID.
    /// </summary>
    /// <param name="id">The GUID of the user to retrieve.</param>
    /// <returns>A <see cref="ServiceResponse{T}"/> containing the matching <see cref="UserDto"/>.</returns>
    /// <response code="200">Successfully retrieved the user.</response>
    /// <response code="404">
    /// The user was not found. Possible <c>Message</c> values:
    /// <list type="bullet">
    ///   <item><c>UserNotFound</c> – No account exists with the given ID.</item>
    /// </list>
    /// </response>
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
    /// Creates a new user account with the provided details.
    /// </summary>
    /// <param name="userCreateDto">The user creation payload. See <see cref="UserCreateDto"/>.</param>
    /// <returns>A <see cref="ServiceResponse{T}"/> containing the newly created <see cref="UserDto"/>.</returns>
    /// <response code="200">
    /// User created successfully. Possible <c>Message</c> values:
    /// <list type="bullet">
    ///   <item><c>UserCreatedSuccess</c> – The account was persisted.</item>
    /// </list>
    /// </response>
    /// <response code="400">
    /// The request payload failed validation. Possible <c>Message</c> values:
    /// <list type="bullet">
    ///   <item><c>EmailAlreadyExists</c> – An account with the provided email is already registered.</item>
    ///   <item><c>PasswordTooWeak</c> – The password does not meet complexity requirements.</item>
    /// </list>
    /// </response>
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
    /// Permanently deletes a user account by their ID.
    /// </summary>
    /// <param name="id">The GUID of the user to delete.</param>
    /// <returns>A <see cref="ServiceResponse{T}"/> containing the deleted <see cref="UserDto"/> on success.</returns>
    /// <response code="200">
    /// User deleted successfully. Possible <c>Message</c> values:
    /// <list type="bullet">
    ///   <item><c>UserDeletedSuccess</c> – The account was removed.</item>
    /// </list>
    /// </response>
    /// <response code="404">
    /// The user was not found. Possible <c>Message</c> values:
    /// <list type="bullet">
    ///   <item><c>UserNotFound</c> – No account exists with the given ID.</item>
    /// </list>
    /// </response>
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
    /// Updates an existing user account's details by their ID.
    /// </summary>
    /// <param name="id">The GUID of the user to update.</param>
    /// <param name="userCreateDto">The updated user payload. See <see cref="UserCreateDto"/>.</param>
    /// <returns>A <see cref="ServiceResponse{T}"/> containing the updated <see cref="UserDto"/>.</returns>
    /// <response code="200">
    /// User updated successfully. Possible <c>Message</c> values:
    /// <list type="bullet">
    ///   <item><c>UserUpdatedSuccess</c> – Changes were persisted.</item>
    /// </list>
    /// </response>
    /// <response code="404">
    /// The user was not found. Possible <c>Message</c> values:
    /// <list type="bullet">
    ///   <item><c>UserNotFound</c> – No account exists with the given ID.</item>
    /// </list>
    /// </response>
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
    /// Retrieves the list of roles assigned to a specific user account.
    /// </summary>
    /// <param name="id">The GUID of the user whose roles to retrieve.</param>
    /// <returns>A <see cref="ServiceResponse{T}"/> containing the list of roles.</returns>
    /// <response code="200">Successfully retrieved the user's roles.</response>
    /// <response code="404">
    /// The user was not found. Possible <c>Message</c> values:
    /// <list type="bullet">
    ///   <item><c>UserNotFound</c> – No account exists with the given ID.</item>
    /// </list>
    /// </response>
    [HttpGet("{id}/roles")]
    [Authorize(Roles = "CHAIR,ADMIN")]
    [ProducesResponseType(typeof(ServiceResponse<UserRolesDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorServiceResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetUserRolesAsync(string id)
    {
        var result = await _userService.GetUserRolesAsync(id);
        return StatusCode(result.StatusCode, result);
    }

    /// <summary>
    /// Assigns one or more roles to a user account.
    /// </summary>
    /// <param name="id">The GUID of the user to assign roles to.</param>
    /// <param name="userRolesDto">The roles payload. See <see cref="UserRolesDto"/>.</param>
    /// <returns>A <see cref="ServiceResponse{T}"/> containing the updated <see cref="UserDto"/> with the new role assignments.</returns>
    /// <response code="200">
    /// Roles assigned successfully. Possible <c>Message</c> values:
    /// <list type="bullet">
    ///   <item><c>RolesAddedSuccess</c> – The specified roles were granted to the user.</item>
    /// </list>
    /// </response>
    /// <response code="404">
    /// The user was not found. Possible <c>Message</c> values:
    /// <list type="bullet">
    ///   <item><c>UserNotFound</c> – No account exists with the given ID.</item>
    /// </list>
    /// </response>
    [HttpPost("{id}/roles/add")]
    [Authorize(Roles = "CHAIR,ADMIN")]
    [ProducesResponseType(typeof(ServiceResponse<UserRolesDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorServiceResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddRolesToUserAsync(string id, [FromBody] UserRolesDto userRolesDto)
    {
        var result = await _userService.AddRolesToUserAsync(id, userRolesDto);
        return StatusCode(result.StatusCode, result);
    }

    /// <summary>
    /// Revokes one or more roles from a user account.
    /// </summary>
    /// <param name="id">The GUID of the user to remove roles from.</param>
    /// <param name="userRolesDto">The roles payload. See <see cref="UserRolesDto"/>.</param>
    /// <returns>A <see cref="ServiceResponse{T}"/> containing the updated <see cref="UserDto"/> reflecting the removed roles.</returns>
    /// <response code="200">
    /// Roles revoked successfully. Possible <c>Message</c> values:
    /// <list type="bullet">
    ///   <item><c>RolesRemovedSuccess</c> – The specified roles were revoked from the user.</item>
    /// </list>
    /// </response>
    /// <response code="404">
    /// The user was not found. Possible <c>Message</c> values:
    /// <list type="bullet">
    ///   <item><c>UserNotFound</c> – No account exists with the given ID.</item>
    /// </list>
    /// </response>
    [HttpPost("{id}/roles/remove")]
    [Authorize(Roles = "CHAIR,ADMIN")]
    [ProducesResponseType(typeof(ServiceResponse<UserRolesDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorServiceResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoveRolesFromUserAsync(string id, [FromBody] UserRolesDto userRolesDto)
    {
        var result = await _userService.RemoveRolesFromUserAsync(id, userRolesDto);
        return StatusCode(result.StatusCode, result);
    }

    /// <summary>
    /// Changes the password for a specific user account.
    /// </summary>
    /// <param name="id">The GUID of the user whose password will be changed.</param>
    /// <param name="changePasswordDto">The password change payload. See <see cref="UserChangePasswordDto"/>.</param>
    /// <returns>A <see cref="ServiceResponse"/> confirming the password was changed.</returns>
    /// <response code="200">
    /// Password changed successfully. Possible <c>Message</c> values:
    /// <list type="bullet">
    ///   <item><c>PasswordChangedSuccess</c> – The new password was saved.</item>
    /// </list>
    /// </response>
    /// <response code="404">
    /// The user was not found. Possible <c>Message</c> values:
    /// <list type="bullet">
    ///   <item><c>UserNotFound</c> – No account exists with the given ID.</item>
    /// </list>
    /// </response>
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
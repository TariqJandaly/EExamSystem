using EExamSystem.Shared.DTOs.Users;
using EExamSystem.Shared.DTOs;
namespace EExamSystem.Api.Interfaces;


/// <summary>
/// Interface for managing users.
/// </summary>
public interface IUserService
{
    Task<ServiceResponse<List<UserDto>>> GetAllUsersAsync();
    Task<ServiceResponse<UserDto>> GetUserAsync(string id);
    Task<ServiceResponse<UserDto>> CreateUserAsync(UserCreateDto userCreateDto);
    Task<ServiceResponse<UserDto>> DeleteUserAsync(string id);
    Task<ServiceResponse<UserDto>> UpdateUserAsync(string id, UserCreateDto userCreateDto);
    Task<ServiceResponse<UserRolesDto>> GetUserRolesAsync(string id);
    Task<ServiceResponse<UserRolesDto>> AddRolesToUserAsync(string id, UserRolesDto userRolesDto);
    Task<ServiceResponse<UserRolesDto>> RemoveRolesFromUserAsync(string id, UserRolesDto userRolesDto);
    Task<ServiceResponse> ChangeUserPasswordAsync(string id, UserChangePasswordDto changePasswordDto);

}
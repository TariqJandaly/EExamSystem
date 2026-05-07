using EExamSystem.Api.Data;
using EExamSystem.Api.Interfaces;
using EExamSystem.Shared.DTOs.Users;
using EExamSystem.Shared.DTOs;
using EExamSystem.Shared.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

public class UserService : IUserService
{
    private readonly AppDbContext _context;
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public UserService(AppDbContext context, UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
    {
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task<ServiceResponse<List<UserDto>>> GetAllUsersAsync()
    {
        var users = await _context.Users.ToListAsync();
        var userDtos = new List<UserDto>();
        foreach (var u in users)
        {
            var roles = await _userManager.GetRolesAsync(u);
            userDtos.Add(new UserDto
            {
                Id = u.Id,
                FullName = u.FullName,
                Email = u.Email,
                Roles = roles.ToList()
            });
        }

        return new ServiceResponse<List<UserDto>>
        {
            Success = true,
            Data = userDtos,
            StatusCode = 200
        };
    }

    public async Task<ServiceResponse<UserDto>> GetUserAsync(string id)
    {
        var user = await _userManager.FindByIdAsync(id);

        if (user == null)
        {
            return new ServiceResponse<UserDto>
            {
                Success = false,
                Message = "UserNotFound",
                StatusCode = 404
            };
        }

        var roles = await _userManager.GetRolesAsync(user);
        return new ServiceResponse<UserDto>
        {
            Success = true,
            Data = new UserDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Roles = roles.ToList()
            },
            StatusCode = 200
        };
    }

    public async Task<ServiceResponse<UserDto>> CreateUserAsync(UserCreateDto userCreateDto)
    {
        var user = new User
        {
            Email = userCreateDto.Email,
            FullName = userCreateDto.FullName,
            UserName = userCreateDto.Email // UserName is required
        };

        // Try to create user
        var result = await _userManager.CreateAsync(user, userCreateDto.Password);
        // If creating process is failed return the errors
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            return new ServiceResponse<UserDto>
            {
                Success = false,
                Message = errors,
                StatusCode = 400
            };
        }

        // add roles
        var roleResult = await _userManager.AddToRolesAsync(user, userCreateDto.Roles);

        if (!roleResult.Succeeded)
        {
            var errors = string.Join(", ", roleResult.Errors.Select(e => e.Description));
            return new ServiceResponse<UserDto>
            {
                Success = false,
                Message = errors,
                StatusCode = 400
            };
        }

        return new ServiceResponse<UserDto>
        {
            Success = true,
            Data = new UserDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email
            },
            StatusCode = 201
        };
    }

    public async Task<ServiceResponse<UserDto>> DeleteUserAsync(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return new ServiceResponse<UserDto>
            {
                Success = false,
                Message = "UserNotFound",
                StatusCode = 404
            };
        }

        var result = await _userManager.DeleteAsync(user);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            return new ServiceResponse<UserDto>
            {
                Success = false,
                Message = errors,
                StatusCode = 400
            };
        }

        return new ServiceResponse<UserDto>
        {
            Success = true,
            Data = new UserDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email
            },
            StatusCode = 200
        };
    }

    public async Task<ServiceResponse<UserDto>> UpdateUserAsync(string id, UserCreateDto userCreateDto)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return new ServiceResponse<UserDto>
            {
                Success = false,
                Message = "UserNotFound",
                StatusCode = 404
            };
        }

        user.FullName = userCreateDto.FullName;
        user.Email = userCreateDto.Email;
        user.UserName = userCreateDto.Email; // Update UserName as well

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            return new ServiceResponse<UserDto>
            {
                Success = false,
                Message = errors,
                StatusCode = 400
            };
        }

        return new ServiceResponse<UserDto>
        {
            Success = true,
            Data = new UserDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email
            },
            StatusCode = 200
        };
    }

    public async Task<ServiceResponse<UserRolesDto>> GetUserRolesAsync(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return new ServiceResponse<UserRolesDto>
            {
                Success = false,
                Message = "UserNotFound",
                StatusCode = 404
            };
        }

        var roles = await _userManager.GetRolesAsync(user);

        return new ServiceResponse<UserRolesDto>
        {
            Success = true,
            Data = new UserRolesDto
            {
                UserId = user.Id,
                Roles = roles.ToList()
            },
            StatusCode = 200
        };
    }

    public async Task<ServiceResponse<UserRolesDto>> AddRolesToUserAsync(string id, UserRolesDto userRolesDto)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return new ServiceResponse<UserRolesDto>
            {
                Success = false,
                Message = "UserNotFound",
                StatusCode = 404
            };
        }

        // Check if roles exist
        foreach (var role in userRolesDto.Roles)
        {
            if (!await _roleManager.RoleExistsAsync(role))
            {
                return new ServiceResponse<UserRolesDto>
                {
                    Success = false,
                    Message = $"RoleNotFound",
                    StatusCode = 400
                };
            }
        }

        // check if user already has any of the roles
        var userRoles = await _userManager.GetRolesAsync(user);
        var duplicateRoles = userRolesDto.Roles.Intersect(userRoles).ToList();
        if (duplicateRoles.Any())
        {
            return new ServiceResponse<UserRolesDto>
            {
                Success = false,
                Message = $"UserAlreadyHasRoles: {string.Join(", ", duplicateRoles)}",
                StatusCode = 400
            };
        }

        var result = await _userManager.AddToRolesAsync(user, userRolesDto.Roles);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            return new ServiceResponse<UserRolesDto>
            {
                Success = false,
                Message = errors,
                StatusCode = 400
            };
        }

        var roles = await _userManager.GetRolesAsync(user);

        return new ServiceResponse<UserRolesDto>
        {
            Success = true,
            Data = new UserRolesDto
            {
                UserId = user.Id,
                Roles = roles.ToList()
            },
            StatusCode = 200
        };
    }

    public async Task<ServiceResponse<UserRolesDto>> RemoveRolesFromUserAsync(string id, UserRolesDto userRolesDto)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return new ServiceResponse<UserRolesDto>
            {
                Success = false,
                Message = "UserNotFound",
                StatusCode = 404
            };
        }

        // Check if roles exist        
        foreach (var role in userRolesDto.Roles)
        {
            if (!await _roleManager.RoleExistsAsync(role))
            {
                return new ServiceResponse<UserRolesDto>
                {
                    Success = false,
                    Message = $"RoleNotFound",
                    StatusCode = 400
                };
            }
        }

        var result = await _userManager.RemoveFromRolesAsync(user, userRolesDto.Roles);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            return new ServiceResponse<UserRolesDto>
            {
                Success = false,
                Message = errors,
                StatusCode = 400
            };
        }

        var roles = await _userManager.GetRolesAsync(user);

        return new ServiceResponse<UserRolesDto>
        {
            Success = true,
            Data = new UserRolesDto
            {
                UserId = user.Id,
                Roles = roles.ToList()
            },
            StatusCode = 200
        };
    }

    public async Task<ServiceResponse> ChangeUserPasswordAsync(string id, UserChangePasswordDto userChangePasswordDto)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return new ServiceResponse
            {
                Success = false,
                Message = "UserNotFound",
                StatusCode = 404
            };
        }

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var result = await _userManager.ResetPasswordAsync(user, token, userChangePasswordDto.newPassword);

        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            return new ServiceResponse
            {
                Success = false,
                Message = errors,
                StatusCode = 400
            };
        }

        return new ServiceResponse
        {
            Success = true,
            Message = "PasswordResetSuccessfully",
            StatusCode = 200
        };
    }
}
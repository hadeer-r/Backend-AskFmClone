using System.Security.Claims;
using AskFm.BLL.DTO.UserDTOs;
using AskFm.DAL.Interfaces;
using AskFm.DAL.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace AskFm.BLL.Services.UserIdentityService;

public class UserService : IUserService
{
    private IUnitOfWork _unitOfWork;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IHttpContextAccessor _httpContextAccessor;


    public UserService(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager,  IHttpContextAccessor httpContextAccessor)
    {
        _unitOfWork = unitOfWork;
        _userManager = userManager;
        _httpContextAccessor = httpContextAccessor;
    }


    public Task<ServiceResult<UpdateUserDTO>> UpdateUserAsync(int userId, UpdateUserDTO updatedUser)
    {
        throw new NotImplementedException();
    }

    public Task<ServiceResult<bool>> DeleteUserAsync(int userId)
    {
        throw new NotImplementedException();
    }

    public Task<ServiceResult<bool>> FollowUserAsync(int followerId, int targetUserId)
    {
        throw new NotImplementedException();
    }

    public Task<ServiceResult<bool>> UnfollowUserAsync(int followerId, int targetUserId)
    {
        throw new NotImplementedException();
    }

    public Task UpdateLastSeenAsync(int userId, DateTime lastSeen)
    {
        throw new NotImplementedException();
    }

    public Task<ServiceResult<ReadUserDTO>> GetUserByIdAsync(int userId)
    {
        throw new NotImplementedException();
    }

    public async Task<ServiceResult<ApplicationUser>> GetCurrentUserAsync()
    {
        string email = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Email).Value;
        if (string.IsNullOrEmpty(email))
        {
            var errors = new List<string>()
            {
                "Can't Access Current user"
            };
            return await ServiceResult<ApplicationUser>.Failure(errors);
        }
        var currentAppUser = await _userManager.FindByEmailAsync(email);

        return await ServiceResult<ApplicationUser>.Success(currentAppUser);
    }

    public Task<ServiceResult<ReadUserDTO>> ResetPassword(string newPassword)
    {
        throw new NotImplementedException();
    }

    public Task<ServiceResult<ReadUserDTO>> ConfirmEmail()
    {
        throw new NotImplementedException();
    }
}
using System.Security.Claims;
using AskFm.BLL.DTO.UserDTOs;
using AskFm.DAL.Interfaces;
using AskFm.DAL.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

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


    public async Task<ServiceResult<bool>> UpdateUserAsync(int userId, UpdateUserDTO updatedUser)
    {
        var res = await CheckNullObjectAsync<bool,UpdateUserDTO>(updatedUser);
        if (!res.success) return res;

        var AppUserToUpdate = await _unitOfWork.Users.GetByIdAsync(userId);
        if (AppUserToUpdate == null)
        {
            return await ServiceResult<bool>.Failure(new List<string> { "User not found" });
        }
        AppUserToUpdate.Name =  updatedUser.Name;
        AppUserToUpdate.Bio =  updatedUser.Bio;
        AppUserToUpdate.AvatarPath = updatedUser.AvatarPath;
        
        await _unitOfWork.Users.UpdateAsync(AppUserToUpdate);
        await _unitOfWork.SaveAsync();
        return await ServiceResult<bool>.Success();
    }

    public async Task<ServiceResult<bool>> DeleteUserAsync(int userId)
    {
        var appUser = await _unitOfWork.Users.GetByIdAsync(userId);
        var res = await CheckNullObjectAsync<bool,ApplicationUser>(appUser);
        if (!res.success) return res;
        
        await _unitOfWork.Users.RemoveAsync(appUser);
        await _unitOfWork.SaveAsync();
        return await ServiceResult<bool>.Success(true);
    }

    public async Task<ServiceResult<bool>> FollowUserAsync(int followerId, int targetUserId)
    {
        
        if (followerId == targetUserId)
        {
            return await ServiceResult<bool>.Failure(new List<string> { "Invalid user" });
        }
        
        await using var transaction = await _unitOfWork.BeginTransactionAsync();
        try
        {
            var userFollower = await _unitOfWork.Users.GetByIdAsync(followerId);
            var userFollowerNullRes = await CheckNullObjectAsync<bool,ApplicationUser>(userFollower);
            if (!userFollowerNullRes.success) return userFollowerNullRes;
            
            var targetUser = await _unitOfWork.Users.GetByIdAsync(targetUserId);
            var targetUserNullRes = await CheckNullObjectAsync<bool,ApplicationUser>(targetUser);
            if (!targetUserNullRes.success) return targetUserNullRes;

            var followExist = await _unitOfWork.Follows.GetAll()
                .FirstOrDefaultAsync(f => f.FollowedId == targetUserId 
                                     && f.FollowerId == followerId);

            if (followExist == null)
            {
                Follow follow = new Follow()
                {
                    FollowerId = followerId,
                    FollowedId = targetUserId,

                };

                userFollower.FollowingCount++;
                targetUser.FollowersCount++;
                await _unitOfWork.Follows.AddAsync(follow);

            }
            else
            {
                followExist.IsActive = true;
                if (followExist.IsDeleted)
                {
                    userFollower.FollowingCount++;
                    targetUser.FollowersCount++;
                    followExist.IsDeleted = false;
                }

                await _unitOfWork.Follows.UpdateAsync(followExist);
            }

            await _unitOfWork.Users.UpdateAsync(userFollower);
            await _unitOfWork.Users.UpdateAsync(targetUser);
            await _unitOfWork.SaveAsync();
            await transaction.CommitAsync();
            return await ServiceResult<bool>.Success(true);
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            return await ServiceResult<bool>.Failure(new List<string>() { "Invalid Follow Operation" });
        }
    }

    public async Task<ServiceResult<bool>> UnfollowUserAsync(int followerId, int targetUserId)
    {
        if (followerId == targetUserId)
        {
            return await ServiceResult<bool>.Failure(new List<string> { "Invalid user" });
        }
        await using var transaction = await _unitOfWork.BeginTransactionAsync();
        try
        {
            var userFollower = await _unitOfWork.Users.GetByIdAsync(followerId);
            var userFollowerNullRes = await CheckNullObjectAsync<bool,ApplicationUser>(userFollower);
            if (!userFollowerNullRes.success) return userFollowerNullRes;
            
            var targetUser = await _unitOfWork.Users.GetByIdAsync(targetUserId);
            var targetUserNullRes = await CheckNullObjectAsync<bool,ApplicationUser>(targetUser);
            if (!targetUserNullRes.success) return targetUserNullRes;
        

            var followExist = await _unitOfWork.Follows.GetAll()
                .FirstOrDefaultAsync(f => f.FollowedId == targetUserId
                                          && f.FollowerId == followerId && !f.IsDeleted);

            if (followExist != null)
            {
                followExist.IsDeleted = true;
                followExist.IsDeleted = true;
                followExist.IsActive = false;
                if (userFollower.FollowingCount > 0) userFollower.FollowingCount--;
                if (targetUser.FollowersCount > 0)   targetUser.FollowersCount--;
                await _unitOfWork.Follows.UpdateAsync(followExist);
                await _unitOfWork.Users.UpdateAsync(userFollower);
                await _unitOfWork.Users.UpdateAsync(targetUser);
                await _unitOfWork.SaveAsync();
            }

            transaction.Commit();

            return await ServiceResult<bool>.Success(true);
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            return await ServiceResult<bool>.Failure(new List<string>() { "Invalid unfollow operation" });
        }
    }

    public async Task<ServiceResult<bool>> UpdateLastSeenAsync(int userId)
    {
        var appUser =await _unitOfWork.Users.GetByIdAsync(userId);
        var res = await CheckNullObjectAsync<bool,ApplicationUser>(appUser);
        if (!res.success) return res;
        await using var  transaction = await _unitOfWork.BeginTransactionAsync();
        try
        {
            appUser.LastSeen = DateTime.Now;
            await _unitOfWork.Users.UpdateAsync(appUser);
            await _unitOfWork.SaveAsync();
            await transaction.CommitAsync();
            return await ServiceResult<bool>.Success(true);
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            return await ServiceResult<bool>.Failure(new List<string>() { "Invalid update operation" });
        }
        
    }

    public async Task<ServiceResult<ReadUserDTO>> GetUserByIdAsync(int userId)
    {
        var user = _unitOfWork.Users.GetById(userId);
        var res = await CheckNullObjectAsync<ReadUserDTO, ApplicationUser>(user);
        if (!res.success) return res;

        return await ServiceResult<ReadUserDTO>.Success(new ReadUserDTO()
        {
            Name = user.Name,
            Email = user.Email,
            LastSeen = user.LastSeen,
            Bio = user.Bio,
            AvatarPath = user.AvatarPath,
            followerCount = user.FollowersCount
        });
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

    public async Task<ServiceResult<bool>> UpdatePassword(int userId, UpdatePasswordDTO updatePasswordDto)
    {
        var nullPassRes = await CheckNullObjectAsync<bool, UpdatePasswordDTO>(updatePasswordDto);
        if(!nullPassRes.success) return nullPassRes;
        var appUser = await _unitOfWork.Users.GetByIdAsync(userId);
        var res = await CheckNullObjectAsync<bool, ApplicationUser>(appUser);
        if (!res.success) return res;
        
        var passwordValid = await _userManager.CheckPasswordAsync(appUser, updatePasswordDto.CurrentPassword);
        if (!passwordValid)
        {
            var errors = new List<string> { "Invalid Password." };
            return await ServiceResult<bool>.Failure(errors);
        }
        if (updatePasswordDto.CurrentPassword==updatePasswordDto.UpdatedPassword)
        {
            var errors = new List<string> { "It is the same old password." };
            return await ServiceResult<bool>.Failure(errors);
        }

        var result = await _userManager.ChangePasswordAsync(appUser, updatePasswordDto.CurrentPassword, updatePasswordDto.UpdatedPassword);
        if (!result.Succeeded)
        {
            var errors = new List<string> { "Cannot Update Current Password." };
            return await ServiceResult<bool>.Failure(errors);
        }
        await _userManager.UpdateAsync(appUser);
        return await ServiceResult<bool>.Success(true);
    }

    public async Task<ServiceResult<ReadUserDTO>> ResetEmail(int userId, string updatedEmail)
    {
        var userApp =await _unitOfWork.Users.GetByIdAsync(userId);
        var res = await CheckNullObjectAsync<ReadUserDTO, ApplicationUser>(userApp);
        if(!res.success) return res;

        //var emailResult = _userManager.GenerateChangeEmailTokenAsync();
        
        //
        throw new NotImplementedException();

    }

    public Task<ServiceResult<ReadUserDTO>> ConfirmEmail()
    {
        throw new NotImplementedException();
    }
    
    
    // Helper check null object
    private async Task<ServiceResult<T>> CheckNullObjectAsync<T,Y>(Y obj, string errorMessage = "Not Found")
    {
        if (obj == null)
        {
            return await ServiceResult<T>.Failure(new List<string>() { errorMessage });
        }
    
        return await ServiceResult<T>.Success();
    }
}
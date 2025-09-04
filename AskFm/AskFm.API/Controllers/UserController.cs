using AskFm.BLL.DTO.UserDTOs;
using AskFm.BLL.Services.UserIdentityService;
using AskFm.DAL;
using AskFm.DAL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AskFm.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(AuthenticationSchemes = "Bearer")]
public class UserController : ControllerBase
{
    private IUnitOfWork _unitOfWork;
    private IAuthService  _authService;
    public IUserService _userService;

    public UserController(IUnitOfWork unitOfWork, IAuthService authService, IUserService userService)
    {
        _unitOfWork = unitOfWork;
        _authService = authService;
        _userService = userService;
    }

    [HttpGet]
    [Route("profile")]
    public async Task<IActionResult> GetCurrentUserAsync()
    {
        var result = await _userService.GetCurrentUserAsync();
        
        if (!result.success)
        {
            return BadRequest(result.Errors);
        }
        ReadUserDTO readUserDTO = new ReadUserDTO()
        {
            Name = result.Data.Name,
            Email = result.Data.Email,
            AvatarPath = result.Data.AvatarPath,
            Bio = result.Data.Bio,
            followerCount = result.Data.FollowersCount,
            LastSeen = result.Data.LastSeen,
        };
        return Ok(readUserDTO);
    }

    [HttpGet]
    [Route("profile/{userId}")]
    public async Task<IActionResult> GetUserAsync(int userId)
    {
        var result = await _userService.GetUserByIdAsync(userId);
        if (!result.success)
        {
            return BadRequest(result.Errors);
        }
        return Ok(result.Data);
    }
    
    [HttpPost]
    [Route("profile/update/{userId}")]
    public async Task<IActionResult> UpdateUserAsync(int userId, UpdateUserDTO updatedUser)
    {
        if (!await _checkCurrentUser(userId))
        {
            return Forbid("Cannot Update this user");
        }
        var result = await _userService.UpdateUserAsync(userId, updatedUser);
        if (!result.success)
        {
            return BadRequest(result.Errors);
        }
        return RedirectToAction("GetUserAsync", new { userId = userId });
    }
    
    [HttpDelete]
    [Route("profile/{userId}")]
    public async Task<IActionResult> DeleteUserAsync(int userId)
    {
        if (!await _checkCurrentUser(userId))
        {
            return Forbid("Cannot Remove this user");
        }
        var result = await _userService.DeleteUserAsync(userId);
        
        if (!result.success)
        {
            return BadRequest(result.Errors);
        }
        return Ok();
    }

    [HttpPost]
    [Route("profile/{followerId}/follow/{targetUserId}")]
    public async Task<IActionResult> FollowUserAsync(int followerId, int targetUserId)
    {
        if (await _checkCurrentUser(targetUserId))
        {
            return Forbid("Cannot Follow the current user");
        }
        if (!await _checkCurrentUser(followerId))
        {
            return Forbid("User can't perform this follow");
        }

        var result = await _userService.FollowUserAsync(followerId, targetUserId);
        if (!result.success)
        {
            return BadRequest(result.Errors);
        }
        return Ok();
    }
    
    [HttpPost]
    [Route("profile/{followerId}/unfollow/{targetUserId}")]
    public async Task<IActionResult> UnFollowUserAsync(int followerId, int targetUserId)
    {
        if (await _checkCurrentUser(targetUserId))
        {
            return Forbid("Cannot unFollow the current user");
        }
        if (!await _checkCurrentUser(followerId))
        {
            return Forbid("User can't perform this unfollow");
        }

        var result = await _userService.UnfollowUserAsync(followerId, targetUserId);
        if (!result.success)
        {
            return BadRequest(result.Errors);
        }
        return Ok();
    }
    
    [HttpPost]
    [Route("profile/update/pass/{userId}")]
    public async Task<IActionResult> UpdatePassword(int userId, string currentPassword, string updatedPassword)
    {
        if (await _checkCurrentUser(userId))
        {
            return Forbid("Cannot update password for another user");
        }
        var result = await _userService.UpdatePassword(userId, currentPassword, updatedPassword);
        if (!result.success)
        {
            return BadRequest(result.Errors);
        }

        return Ok();
    }
    
    
    //-------------------------------------------------------------------
    // Helper functions
    private async Task<bool> _checkCurrentUser(int userId)
    {
        var current_user = _userService.GetCurrentUserAsync();
        return current_user.Id == userId;
    }

    
    /* TODO 
    update email
    confirm email
    check user not deleted in login
    
    */   
    
}
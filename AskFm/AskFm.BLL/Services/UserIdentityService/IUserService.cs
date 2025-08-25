using AskFm.BLL.DTO.UserDTOs;

namespace AskFm.BLL.Services.UserIdentityService;

public interface IUserService
{
    Task<ServiceResult<UpdateUserDTO>> UpdateUserAsync(int userId, UpdateUserDTO updatedUser);
    Task<ServiceResult<bool>> DeleteUserAsync(int userId);
    Task<ServiceResult<bool>> FollowUserAsync(int followerId, int targetUserId);
    Task<ServiceResult<bool>> UnfollowUserAsync(int followerId, int targetUserId);
    Task UpdateLastSeenAsync(int userId, DateTime lastSeen);
    Task<ServiceResult<ReadUserDTO>> GetUserByIdAsync(int userId);
    Task<ServiceResult<ReadUserDTO>> GetCurrentUserAsync();
    Task<ServiceResult<ReadUserDTO>> ResetPassword(string newPassword);
    Task<ServiceResult<ReadUserDTO>> ConfirmEmail();
    
    
    /*
GET Users only for now
 getUserbyId
 EditUser
 DeleteUser
 FollowUser
 unfollowUser
 reset password
 confirm email
 Helper Function: getCurrentUserId
*/   
}

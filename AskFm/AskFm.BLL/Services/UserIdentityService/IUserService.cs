using AskFm.BLL.DTO.UserDTOs;

namespace AskFm.BLL.Services.UserIdentityService;

public interface IUserService
{
    Task<ServiceResult<UpdateUserDTO>> UpdateUserAsync(int userId, UpdateUserDTO updatedUser);
    Task<ServiceResult<bool>> DeleteUserAsync(int userId);
    Task<ServiceResult<bool>> FollowUserAsync(int followerId, int targetUserId);
    Task<ServiceResult<bool>> UnfollowUserAsync(int followerId, int targetUserId);
    Task UpdateLastSeenAsync(int userId, DateTime lastSeen);
    Task<ReadUserDTO> GetUserByIdAsync(int userId);
}

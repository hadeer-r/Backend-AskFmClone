using AskFm.BLL.DTO.UserDTOs;

namespace AskFm.BLL.Services.UserIdentityService;

public class UserService : IUserService
{
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

    public Task<ReadUserDTO> GetUserByIdAsync(int userId)
    {
        throw new NotImplementedException();
    }
}
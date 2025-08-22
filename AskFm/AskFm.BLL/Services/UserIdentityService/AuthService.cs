using AskFm.BLL.DTO.UserDTOs;

namespace AskFm.BLL.Services.UserIdentityService;

public class AuthService : IAuthService
{
    public Task<ServiceResult<LoginDTO>> LoginAsync(LoginDTO request)
    {
        throw new NotImplementedException();
    }

    public Task<ServiceResult<RegisterUserDTO>> RegisterAsync(RegisterUserDTO request)
    {
        throw new NotImplementedException();
    }

    public Task<ServiceResult<bool>> RevokeRefreshTokenAsync(string refreshToken)
    {
        throw new NotImplementedException();
    }

    public Task<ServiceResult<bool>> RevokeAllRefreshTokensAsync(int userId)
    {
        throw new NotImplementedException();
    }
}
using AskFm.BLL.DTO.UserDTOs;

namespace AskFm.BLL.Services.UserIdentityService;

public interface IAuthService
{
    public Task<ServiceResult<LoginDTO>> LoginAsync(LoginDTO request);
    public Task<ServiceResult<RegisterUserDTO>> RegisterAsync(RegisterUserDTO request);
    // Task<ServiceResult<RefreshTokenResult>> RefreshTokenAsync(string refreshToken);
    public Task<ServiceResult<bool>> RevokeRefreshTokenAsync(string refreshToken);
    public Task<ServiceResult<bool>> RevokeAllRefreshTokensAsync(int userId);
}
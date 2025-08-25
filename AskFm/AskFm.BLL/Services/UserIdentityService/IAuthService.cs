using AskFm.BLL.DTO.UserDTOs;
using AskFm.DAL.Models;

namespace AskFm.BLL.Services.UserIdentityService;

public interface IAuthService
{
    public Task<ServiceResult<AuthResponseDTO>> LoginAsync(LoginDTO request);
    public Task<ServiceResult<AuthResponseDTO>> RegisterAsync(RegisterUserDTO request);
    Task<ServiceResult<AuthResponseDTO>> RefreshTokenAsync(int id, string refreshToken);
    public Task<ServiceResult<bool>> RevokeRefreshTokenAsync(int id, string refreshToken);
    public void Logout();
}
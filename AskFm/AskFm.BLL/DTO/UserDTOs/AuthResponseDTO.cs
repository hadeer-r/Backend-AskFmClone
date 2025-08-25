using AskFm.DAL.Models;

namespace AskFm.BLL.DTO.UserDTOs;

public class AuthResponseDTO
{
    public bool IsAuthenticated { get; set; }
    public string Token { get; set; }
    public RefreshToken RefreshToken { get; set; }
    public ReadUserDTO User { get; set; }
}
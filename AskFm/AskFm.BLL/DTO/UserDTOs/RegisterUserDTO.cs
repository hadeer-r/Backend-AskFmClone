namespace AskFm.BLL.DTO.UserDTOs;

public class RegisterUserDTO
{
    public string Name { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public string Bio { get; set; }
    public string AvatarPath { get; set; }
    public string Passwrod { get; set; }
    public DateTime LastSeen { get; set; }
}
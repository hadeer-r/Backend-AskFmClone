using System.ComponentModel.DataAnnotations;

namespace AskFm.BLL.DTO.UserDTOs;

public class ForgotPasswordDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }   
}
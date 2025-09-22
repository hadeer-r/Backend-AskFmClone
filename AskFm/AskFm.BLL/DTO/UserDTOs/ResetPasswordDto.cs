using System.ComponentModel.DataAnnotations;

namespace AskFm.BLL.DTO.UserDTOs;

public class ResetPasswordDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    public string Token { get; set; }

    [Required]
    public string NewPassword { get; set; }

    [Required]
    [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
    public string ConfirmPassword { get; set; }
}
using System.ComponentModel.DataAnnotations;

namespace AskFm.BLL.DTO;

public class EmailSettings
{
    [Required, EmailAddress]
    public string From {get; set;}
    [Required]
    public string Client {get; set;}
    [Required]
    public string Password {get;set;}
    [Range(1, 65535)]
    public int Port {get; set; }
    
}
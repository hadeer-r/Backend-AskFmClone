using Microsoft.EntityFrameworkCore;

namespace AskFm.BLL.DTO;
public class RefreshTokenDto
{
    public string Token { get; set; }
    public DateTime ExpireOn { get; set; }
    public bool IsExpired => DateTime.Now >= ExpireOn;
    public int ExpireAfter { get; set; }
    public DateTime CreatedOn { get; set; }
}
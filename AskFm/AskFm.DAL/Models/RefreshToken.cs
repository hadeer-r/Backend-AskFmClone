using Microsoft.EntityFrameworkCore;

namespace AskFm.DAL.Models;
[Owned]
public class RefreshToken
{
    public string Token { get; set; }
    public DateTime ExpireOn { get; set; }
    public bool IsExpired => DateTime.Now >= ExpireOn;
    public DateTime? RevokedOn { get; set; }
    public bool IsActive => RevokedOn == null && !IsExpired;
    
    public DateTime CreatedOn { get; set; }
}
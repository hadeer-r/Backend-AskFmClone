namespace AskFm.BLL.Services.UserIdentityService;

public class JwtOptions
{
    public string Issuer { get; set; }
    public string  Audience { get; set; }
    public string SigningKey  { get; set; }
}
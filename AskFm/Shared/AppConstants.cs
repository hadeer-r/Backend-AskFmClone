namespace Shared;

public class AppConstants
{

    public static string UserJwtCacheKey(int userId)
    {
        return $"userId:{userId}:current_jti";
    }

    public static string JwtCacheKey(string id)
    {
        return $"jti:{id}";
    }
    
    public static string UserRefreshTokenCacheKey(int userId)
    {
        return $"refresh_token:userId:{userId}";
    }
}
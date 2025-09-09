using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace AskFm.BLL.Services;

public interface IRedisService
{
    public Task SetCacheAsync<T>(string key, T value, TimeSpan expirationTime);

    public Task<T?> GetCacheAsync<T>(string key);

    public Task RemoveCacheAsync(string key);
}


using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;

namespace Cachara.Users.API.Infrastructure.Cache;

public class CacheService(IDistributedCache cache, ILogger<CacheService> logger) : ICacheService
{
    private readonly JsonSerializerOptions _jsonOptions = new(JsonSerializerDefaults.Web);

    public async Task<T> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            var data = await cache.GetStringAsync(key, cancellationToken);
            if (data is null)
            {
                logger.LogDebug("Cache miss for key: {Key}", key);
                return default;
            }

            logger.LogDebug("Cache hit for key: {Key}", key);
            return JsonSerializer.Deserialize<T>(data, _jsonOptions);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting key from cache: {Key}", key);
            return default;
        }
    }

    public async Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default)
    {
        var data = await cache.GetStringAsync(key, cancellationToken);
        var exists = data is not null;
        logger.LogDebug("Cache key {Key} exists: {Exists}", key, exists);
        return exists;
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null, CancellationToken cancellationToken = default)
    {
        try
        {
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiry ?? TimeSpan.FromMinutes(30)
            };

            var json = JsonSerializer.Serialize(value, _jsonOptions);
            await cache.SetStringAsync(key, json, options, cancellationToken);
            logger.LogDebug("Cached key: {Key} with expiry: {Expiry}", key, options.AbsoluteExpirationRelativeToNow);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error setting key in cache: {Key}", key);
        }
    }

    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            await cache.RemoveAsync(key, cancellationToken);
            logger.LogDebug("Removed key: {Key} from cache", key);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error removing key from cache: {Key}", key);
        }
    }
}

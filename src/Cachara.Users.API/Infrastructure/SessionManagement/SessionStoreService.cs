using System.Security.Claims;
using Cachara.Shared.Infrastructure;
using Cachara.Users.API.API.Authentication;
using Cachara.Users.API.Domain.Entities;
using Cachara.Users.API.Infrastructure.Cache;
using Cachara.Users.API.Services.Abstractions;
using FluentResults;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace Cachara.Users.API.Infrastructure.SessionManagement;

public class SessionStoreService : ISessionStoreService<UserAccount>
{
    private readonly ICacheService _cache;
    private readonly ILogger<SessionStoreService> _logger;

    public SessionStoreService(ICacheService cache, ILogger<SessionStoreService> logger)
    {
        _cache = cache;
        _logger = logger;
    }


    // TODO: manage session accordingly
    public async Task<string> CreateSession(UserAccount account)
    {
        var sessionId = Guid.NewGuid().ToString();
        var sessionData = new SessionData
        {
            UserId = account.Id,
            Claims = account.Claims,
            ExpiresAt = DateTime.UtcNow.AddMinutes(30)
        };

        await _cache.SetAsync(sessionId, JsonConvert.SerializeObject(sessionData),TimeSpan.FromMinutes(30));

        return sessionId;

        // var tokenData = _tokenProvider.Decode(token); // Incorrect
        // //await _cacheProvider.SetAsync($"token:{token}", user, tokenData.ExpiresAt - DateTimeOffset.UtcNow);
        //
        // var sessions = await _cacheProvider.GetAsync<List<string>>($"user-sessions:{user.Id}") ?? new List<string>();
        // sessions.Add($"token:{token}");
        // await _cacheProvider.SetAsync($"user-sessions:{user.Id}", sessions);
    }

    public async Task<UserAccount?> GetSession(string sessionId)
    {
        var sessionJson = await _cache.GetAsync<string>(sessionId);
        return sessionJson == null
            ? null
            : JsonConvert.DeserializeObject<UserAccount>(sessionJson);
    }

    public async Task InvalidateSession(string sessionId)
    {
        await _cache.RemoveAsync(sessionId);

        // await _cacheProvider.RemoveAsync($"token:{token}");
        //
        // var sessions = await _cacheProvider.GetAsync<List<string>>($"user-sessions:{user.Id}");
        // sessions?.Remove($"token:{token}");
        // await _cacheProvider.SetAsync($"user-sessions:{user.Id}", sessions);
    }

    public Task InvalidateAllSessionsAsync(string accountId)
    {
        throw new NotImplementedException();
        // var sessions = await _cacheProvider.GetAsync<List<string>>($"user-sessions:{userId}");
        // if (sessions == null) return;
        //
        // foreach (var sessionId in sessions)
        // {
        //     await _cacheProvider.RemoveAsync($"session:{sessionId}");
        // }
        //
        // await _cacheProvider.RemoveAsync($"user-sessions:{userId}");
    }

    public Task<bool> IsSessionActiveAsync(string sessionId)
    {
        throw new NotImplementedException();
        //return await _cacheProvider.ExistsAsync(sessionId);
    }
}

public class SessionData
{
    public string UserId { get; set; }
    public IEnumerable<Claim> Claims { get; set; }
    public DateTimeOffset ExpiresAt { get; set; }
}

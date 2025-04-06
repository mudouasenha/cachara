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
    public async Task<SessionData> CreateSession(UserAccount account)
    {
        var sessionId = Guid.NewGuid().ToString();
        var sessionData = new SessionData
        {
            Id = sessionId,
            UserId = account.Id,
            Claims = account.Claims,
            ExpiresAt = DateTime.UtcNow.AddMinutes(30)
        };

        await _cache.SetAsync(
            $"user-sessions:{account.Id}",
            JsonConvert.SerializeObject(sessionId),
            TimeSpan.FromMinutes(30));

        await _cache.SetAsync($"session:{sessionId}", sessionData);

        return sessionData;
    }

    public async Task<SessionData?> GetSession(string sessionId)
    {
        var sessions = await _cache.GetAsync<List<string>>($"user-sessions:{sessionId}") ?? new List<string>();

        var sessionData = sessions?.FirstOrDefault(s => s.Equals(sessionId, StringComparison.OrdinalIgnoreCase));

        return sessionData == null
            ? null
            : JsonConvert.DeserializeObject<SessionData>(sessionData);
    }

    public async Task InvalidateSession(string sessionId)
    {
        await _cache.RemoveAsync($"user-sessions:{sessionId}");
        await _cache.RemoveAsync($"session:{sessionId}");
    }

    public async Task InvalidateAllSessionsAsync(string accountId)
    {
        var sessions = await _cache.GetAsync<List<string>>($"user-sessions:{accountId}");
        if (sessions == null) return;

        foreach (var sessionId in sessions)
        {
            await _cache.RemoveAsync($"session:{sessionId}");
        }

        await _cache.RemoveAsync($"user-sessions:{accountId}");
    }

    public async Task<bool> IsSessionActiveAsync(string sessionId)
    {
        return await _cache.ExistsAsync(sessionId);
    }
}



using Cachara.Shared.Application.Abstractions;
using Cachara.Shared.Domain.Entities;
using Cachara.Shared.Infrastructure;
using Cachara.Shared.Infrastructure.Session;
using Cachara.Users.API.API.Authentication;
using Cachara.Users.API.Infrastructure.Cache;
using Microsoft.Extensions.Logging;

namespace Cachara.Users.API.Infrastructure.SessionManagement;

// TODO: IMPLEMENT CACHE-ASIDE PATTERN FOR SESSION STORAGE
public class SessionStoreService : ISessionStoreService<UserAccount>
{
    private readonly ICacheService _cache;
    private readonly ILogger<SessionStoreService> _logger;

    public SessionStoreService(ICacheService cache, ILogger<SessionStoreService> logger)
    {
        _cache = cache;
        _logger = logger;
    }

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

        _logger.LogInformation("Creating new session. UserId: {UserId}, SessionId: {SessionId}", account.Id, sessionId);

        await _cache.SetAsync($"session:{sessionId}", sessionData, TimeSpan.FromMinutes(30));

        var userSessions = await _cache.GetAsync<List<string>>($"user-sessions:{account.Id}") ?? new List<string>();
        userSessions.Add(sessionId);
        await _cache.SetAsync($"user-sessions:{account.Id}", userSessions, TimeSpan.FromMinutes(30));

        _logger.LogDebug("Session created and stored. UserId: {UserId}, TotalSessions: {SessionCount}", account.Id, userSessions.Count);

        return sessionData;
    }

    public async Task<SessionData> GetSession(string sessionId)
    {
        var sessionData = await _cache.GetAsync<SessionData>($"session:{sessionId}");
        if (sessionData == null)
        {
            _logger.LogWarning("Session not found. SessionId: {SessionId}", sessionId);
        }
        else
        {
            _logger.LogDebug("Session retrieved. SessionId: {SessionId}", sessionId);
        }

        return sessionData;
    }

    public async Task<List<SessionData>> GetSessions(string userId)
    {
        var sessionIds = await _cache.GetAsync<List<string>>($"user-sessions:{userId}") ?? new List<string>();

        var sessions = new List<SessionData>();
        foreach (var sessionId in sessionIds)
        {
            var sessionData = await _cache.GetAsync<SessionData>($"session:{sessionId}");
            if (sessionData != null)
            {
                sessions.Add(sessionData);
            }
        }

        _logger.LogInformation("Retrieved {SessionCount} sessions for UserId: {UserId}", sessions.Count, userId);

        return sessions;
    }

    public async Task InvalidateSession(string sessionId)
    {
        var sessionData = await _cache.GetAsync<SessionData>($"session:{sessionId}");
        if (sessionData != null)
        {
            var userSessions = await _cache.GetAsync<List<string>>($"user-sessions:{sessionData.UserId}") ?? new List<string>();
            userSessions.Remove(sessionId);

            if (userSessions.Count > 0)
            {
                await _cache.SetAsync($"user-sessions:{sessionData.UserId}", userSessions, TimeSpan.FromMinutes(30));
            }
            else
            {
                await _cache.RemoveAsync($"user-sessions:{sessionData.UserId}");
            }

            _logger.LogDebug("Session removed from user's session list. SessionId: {SessionId}", sessionId);
        }

        await _cache.RemoveAsync($"session:{sessionId}");
        _logger.LogInformation("Session invalidated. SessionId: {SessionId}", sessionId);
    }

    public async Task InvalidateAllSessionsAsync(string userId)
    {
        var sessionIds = await _cache.GetAsync<List<string>>($"user-sessions:{userId}");
        if (sessionIds != null)
        {
            foreach (var sessionId in sessionIds)
            {
                await _cache.RemoveAsync($"session:{sessionId}");
            }

            await _cache.RemoveAsync($"user-sessions:{userId}");
            _logger.LogInformation("All sessions invalidated for UserId: {UserId}", userId);
        }
        else
        {
            _logger.LogDebug("No active sessions found to invalidate for UserId: {UserId}", userId);
        }
    }

    public async Task<bool> IsSessionActiveAsync(string sessionId)
    {
        var exists = await _cache.ExistsAsync($"session:{sessionId}");
        _logger.LogDebug("Session active check. SessionId: {SessionId}, IsActive: {IsActive}", sessionId, exists);
        return exists;
    }
}

using Cachara.Users.API.API.Authentication;
using Cachara.Users.API.Domain.Entities;
using Cachara.Users.API.Infrastructure.Cache;
using Cachara.Users.API.Services.Abstractions;
using FluentResults;

namespace Cachara.Users.API.Infrastructure.SessionManagement;

public class SessionService : ISessionService
{
    private readonly IJwtProvider _tokenProvider;
    private readonly ICacheService _cacheProvider; // e.g., Redis
    private readonly ILogger<SessionService> _logger;

    public SessionService(
        IJwtProvider tokenProvider,
        ICacheService cacheProvider,
        ILogger<SessionService> logger)
    {
        _tokenProvider = tokenProvider;
        _cacheProvider = cacheProvider;
        _logger = logger;
    }

    public async Task CreateSessionAsync(UserAccount user, string token)
    {
        var tokenData = _tokenProvider.Decode(token); // Incorrect
        await _cacheProvider.SetAsync($"token:{token}", user, tokenData.ExpiresAt - DateTimeOffset.UtcNow);

        var sessions = await _cacheProvider.GetAsync<List<string>>($"user-sessions:{user.Id}") ?? new List<string>();
        sessions.Add($"token:{token}");
        await _cacheProvider.SetAsync($"user-sessions:{user.Id}", sessions);
    }

    public async Task<bool> IsSessionActiveAsync(string sessionId)
    {
        return await _cacheProvider.ExistsAsync(sessionId);
    }

    public async Task RevokeSessionAsync(UserAccount user, string token)
    {
        await _cacheProvider.RemoveAsync($"token:{token}");

        var sessions = await _cacheProvider.GetAsync<List<string>>($"user-sessions:{user.Id}");
        sessions?.Remove($"token:{token}");
        await _cacheProvider.SetAsync($"user-sessions:{user.Id}", sessions);
    }

    public async Task RevokeUserSessionsAsync(string userId)
    {
        var sessions = await _cacheProvider.GetAsync<List<string>>($"user-sessions:{userId}");
        if (sessions == null) return;

        foreach (var sessionId in sessions)
        {
            await _cacheProvider.RemoveAsync($"session:{sessionId}");
        }

        await _cacheProvider.RemoveAsync($"user-sessions:{userId}");
    }
}

public interface ISessionService
{
    Task CreateSessionAsync(UserAccount user, string token);
    Task<bool> IsSessionActiveAsync(string sessionId);
    Task RevokeSessionAsync(UserAccount user, string token);
    Task RevokeUserSessionsAsync(string userId);
}

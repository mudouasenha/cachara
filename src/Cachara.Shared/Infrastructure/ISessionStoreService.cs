namespace Cachara.Shared.Infrastructure;

public interface ISessionStoreService<TAccount> where TAccount : IAccount
{
    Task<SessionData> CreateSession(TAccount account);
    Task<SessionData> GetSession(string sessionId);
    Task InvalidateSession(string sessionId);

    Task InvalidateAllSessionsAsync(string accountId);
    Task<bool> IsSessionActiveAsync(string sessionId);

}

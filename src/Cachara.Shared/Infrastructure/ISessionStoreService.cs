namespace Cachara.Shared.Infrastructure;

public interface ISessionStoreService<TAccount> where TAccount : IAccount
{
    Task<string> CreateSession(TAccount account);
    Task<TAccount?> GetSession(string sessionId);
    Task InvalidateSession(string sessionId);

    Task InvalidateAllSessionsAsync(string accountId);
    Task<bool> IsSessionActiveAsync(string sessionId);

}

using System.Linq.Expressions;

namespace Cachara.Shared.Infrastructure.Hangfire
{
    public interface IBackgroundServiceManager
    {
        string Enqueue<T>(Expression<Action<T>> methodCall);
    }
}

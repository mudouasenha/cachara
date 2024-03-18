using System.Linq.Expressions;

namespace Cachara.API.Hangfire
{
    public interface IBackgroundServiceManager
    {
        string Enqueue<T>(Expression<Action<T>> methodCall);
    }
}

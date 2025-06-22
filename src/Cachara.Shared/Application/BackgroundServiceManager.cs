using System.Linq.Expressions;
using Cachara.Shared.Infrastructure.Hangfire;
using Hangfire;

namespace Cachara.Shared.Application;

public class BackgroundServiceManager(IBackgroundJobClient backgroundJobClient) : IBackgroundServiceManager
{
    public string Enqueue<T>(Expression<Action<T>> methodCall)
    {
        return backgroundJobClient.Enqueue(methodCall);
    }
}

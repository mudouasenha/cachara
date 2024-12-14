using System.Linq.Expressions;
using Cachara.Shared.Abstractions;
using Cachara.Shared.Infrastructure.Hangfire;
using Hangfire;

namespace Cachara.Services.Services
{
    public class BackgroundServiceManager : IBackgroundServiceManager
    {
        private readonly IBackgroundJobClient backgroundJobClient;

        public BackgroundServiceManager(IBackgroundJobClient backgroundJobClient)
        {
            this.backgroundJobClient = backgroundJobClient;
        }
        public string Enqueue<T>(Expression<Action<T>> methodCall)
        {
            return backgroundJobClient.Enqueue(methodCall);
        }
    }
}

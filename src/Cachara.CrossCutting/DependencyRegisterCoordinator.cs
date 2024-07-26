using Cachara.Data;
using Cachara.Domain.Interfaces.Services;
using Cachara.Services;
using Cachara.Services.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Cachara.CrossCutting
{
    public static class DependencyRegisterCoordinator
    {
        public static IServiceCollection AddCrossCutting(this IServiceCollection services, IConfiguration configuration)
        {
            services
                .AddCacharaData(configuration)
                .AddServices(configuration);
            
            return services;
        }
    }
}
using Cachara.Domain.Abstractions.Security;
using Cachara.Domain.Interfaces.Services;
using Cachara.Services.Internal;
using Cachara.Services.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Cachara.Services;

public static class DiCacharaServices
{
        public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
        {
            services
                .AddScoped<IPostService, PostService>()
                .AddScoped<IUserService, UserService>()
                .AddScoped<IPostManagerService, PostManagerService>();
            
            
            return services;
        }
}
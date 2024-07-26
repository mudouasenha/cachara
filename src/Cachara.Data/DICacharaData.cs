using Cachara.Data.EF;
using Cachara.Data.Interfaces;
using Cachara.Data.Persistence.Connections;
using Cachara.Data.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Cachara.Data
{
    public static class DICacharaData
    {
        public static IServiceCollection AddCacharaData(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient(typeof(IRepository<>), typeof(BaseRepository<>));
            services.AddTransient(typeof(IReadRepository<>), typeof(BaseRepository<>));
            services.AddTransient<IPostRepository, PostRepository>();
            services.AddTransient<IUserRepository, UserRepository>();

            return services;
        }
    }
}
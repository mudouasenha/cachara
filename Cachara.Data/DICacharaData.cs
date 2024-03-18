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
            services.AddDbContext<ApplicationContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            services.AddScoped<IApplicationContext>(provider => provider.GetRequiredService<ApplicationContext>());

            services.AddScoped<IApplicationWriteDbConnection, ApplicationWriteDbConnection>();
            services.AddScoped<IApplicationReadDbConnection, ApplicationReadDbConnection>();

            services.AddTransient(typeof(IRepository<>), typeof(BaseRepository<>));
            services.AddTransient(typeof(IReadRepository<>), typeof(BaseRepository<>));
            services.AddTransient<IPostRepository, PostRepository>();

            return services;
        }
    }
}
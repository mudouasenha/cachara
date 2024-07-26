using System.Reflection;
using Cachara.API.Extensions;
using Cachara.API.Infrastructure;
using Cachara.API.Options;
using Cachara.CrossCutting;
using Cachara.Data.EF;
using Cachara.Data.Interfaces;
using Cachara.Data.Persistence.Connections;
using Flurl;
using Hellang.Middleware.ProblemDetails;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Cachara.API
{
    public partial class CacharaService<TOptions> where TOptions : CacharaOptions, new()
    {
        private IConfiguration Configuration;

        private IHostEnvironment Environment;

        private TOptions Options { get; set; }
        
        public CacharaService(IHostEnvironment environment, IConfiguration configuration)
        {
            Environment = environment;
            Configuration = configuration;
            Options = new TOptions()
            {
                Name = GetType().Name
            };
            try
            {
                Configuration?.Bind(Options);
            }
            catch (Exception)
            {
                Console.WriteLine("Could not Bind Options");
                throw;
            }
        }
        
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCrossCutting(Configuration);
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            
            services.AddProblemDetails();
            services.AddControllers(options =>
            {
                options.Conventions.Add(new ApiExplorerGroupPerVersionConvention());

                options.InputFormatters.Add(new TextPlainInputFormatter());
                options.InputFormatters.Add(new StreamInputFormatter());
            });
            
            services.AddResponseCaching();

            services.AddEndpointsApiExplorer();
            services.AddCustomSwagger();
            ConfigureDataAccess(services);
        }

        public void ConfigureDataAccess(IServiceCollection services)
        {
            services.AddDbContext<ApplicationContext>(options =>
            {
                options.UseSqlServer(Options.SqlDb);
                options.UseQueryTrackingBehavior((QueryTrackingBehavior.NoTracking));
                options.EnableSensitiveDataLogging(Environment.IsDevelopment());
            });

            services.AddScoped<IApplicationContext>(provider => provider.GetRequiredService<ApplicationContext>());

            services.AddScoped<IApplicationWriteDbConnection, ApplicationWriteDbConnection>();
            services.AddScoped<IApplicationReadDbConnection, ApplicationReadDbConnection>();
        }
        
        public void ConfigureApp(IApplicationBuilder app)
        {
            app.UseSwaggerUI(opts =>
            {
                opts.EnableTryItOutByDefault();
                opts.EnablePersistAuthorization();
                opts.DisplayRequestDuration();
                
                var swaggerGenOptions = app.ApplicationServices.GetService<SwaggerGeneratorOptions>();

                foreach (var swaggerDoc in swaggerGenOptions.SwaggerDocs)
                {
                    var swaggerPathBase = "/swagger";

                    opts.SwaggerEndpoint(
                        swaggerPathBase.AppendPathSegment($"/{swaggerDoc.Key}/swagger.json"), swaggerDoc.Key);
                }
            });

            app.UseProblemDetails();
            app.UseHttpsRedirection();
            
            app.UseResponseCaching();
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapSwagger();
            });
        }
        
        public virtual void Configure(IApplicationBuilder app)
        {
            ConfigureApp(app);
        }
    }
}

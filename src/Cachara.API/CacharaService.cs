using System.Reflection;
using System.Text.Json.Serialization;
using Cachara.API.Extensions;
using Cachara.API.Hangfire;
using Cachara.API.Infrastructure;
using Cachara.API.Options;
using Cachara.CrossCutting;
using Cachara.Data.EF;
using Cachara.Data.Interfaces;
using Cachara.Data.Persistence.Connections;
using Cachara.Services.Services;
using Flurl;
using Hangfire;
using Hangfire.Console;
using Hangfire.SqlServer;
using Hellang.Middleware.ProblemDetails;
using Hellang.Middleware.ProblemDetails.Mvc;
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
                Console.WriteLine($"Could not Bind Options for {nameof(CacharaService<TOptions>)}");
                throw;
            }
        }
        
        public void ConfigureServices(IServiceCollection services)
        {
            // Dependency Injection Options
            OptionsServiceCollectionExtensions.AddOptions<TOptions>(services).Bind(Configuration);
            services.AddCrossCutting(Configuration);
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            
            services.AddProblemDetails(delegate (Hellang.Middleware.ProblemDetails.ProblemDetailsOptions opts) { });
            
            services.AddControllers(options =>
            {
                options.Conventions.Add(new ApiExplorerGroupPerVersionConvention());

                options.InputFormatters.Add(new TextPlainInputFormatter());
                options.InputFormatters.Add(new StreamInputFormatter());
            }).AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.Never;
            })
            .AddProblemDetailsConventions();

            services.AddOpenApiDocument();
            services.AddResponseCaching();
            services.AddEndpointsApiExplorer();
            services.AddCustomSwagger();
            
            ConfigureHangfire(services);
            ConfigureDataAccess(services);
        }
        
        public void ConfigureHangfire(IServiceCollection services)
        {
            services.AddScoped<IBackgroundServiceManager, BackgroundServiceManager>();

            services.AddHangfire((provider, config) =>
            {
                config.UseSimpleAssemblyNameTypeSerializer();
                config.SetDataCompatibilityLevel(CompatibilityLevel.Version_180);
                config.UseRecommendedSerializerSettings(x =>
                {
                    //x.Converters.Add(new JsonDateOnlyConverter());
                });
                config.UseSqlServerStorage(Options.JobsSqlDb, new SqlServerStorageOptions()
                {
                    SchemaName = "CacharaHangfire",
                    PrepareSchemaIfNecessary = true
                });
                config.UseConsole();
            });
            
            int totalWorkerCount = System.Environment.ProcessorCount * 20;
            services.AddHangfireServer(options =>
            {
                options.WorkerCount = totalWorkerCount;
                options.Queues = new string[]
                {
                    "default"
                };
            });
        }
        
        public void ConfigureDataAccess(IServiceCollection services)
        {
            services.AddDbContext<CacharaSocialDbContext>(options =>
            {
                options.UseSqlServer(Options.SqlDb);
                options.UseQueryTrackingBehavior((QueryTrackingBehavior.NoTracking));
                options.EnableSensitiveDataLogging(Environment.IsDevelopment());
            }).AddAsyncInitializer<DbContextInitializer<CacharaSocialDbContext>>()
            .AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<CacharaSocialDbContext>());;
        }
        
        public void ConfigureApp(IApplicationBuilder app)
        {
            app.UseProblemDetails();
            app.UseOpenApi();
            app.UseSwaggerUI(opts =>
            {
                opts.EnableTryItOutByDefault();
                opts.EnablePersistAuthorization();
                opts.DisplayRequestDuration();
                opts.DefaultModelsExpandDepth(-1);
                
                var swaggerGenOptions = app.ApplicationServices.GetService<SwaggerGeneratorOptions>();

                foreach (var swaggerDoc in swaggerGenOptions.SwaggerDocs)
                {
                    var swaggerPathBase = "/swagger";

                    opts.SwaggerEndpoint(
                        swaggerPathBase.AppendPathSegment($"/{swaggerDoc.Key}/swagger.json"), swaggerDoc.Key);
                }
            });

            
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

            app.UseHangfireDashboard();
        }
        
        public virtual void Configure(IApplicationBuilder app)
        {
            ConfigureApp(app);
        }
    }
}

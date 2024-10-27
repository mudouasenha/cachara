using System.Reflection;
using System.Text.Json.Serialization;
using Cachara.API.Hangfire;
using Cachara.Content.API.API.Hangfire;
using Cachara.Content.API.Extensions;
using Cachara.Content.API.Infrastructure;
using Cachara.Content.API.Infrastructure.Data;
using Cachara.Content.API.Infrastructure.Data.Repository;
using Cachara.Content.API.Options;
using Cachara.Content.API.Services;
using Cachara.Data.Interfaces;
using Cachara.Domain.Abstractions.Security;
using Cachara.Services;
using Cachara.Services.Internal;
using Cachara.Services.Security;
using Flurl;
using Hangfire;
using Hangfire.Console;
using Hangfire.SqlServer;
using Hellang.Middleware.ProblemDetails;
using Hellang.Middleware.ProblemDetails.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Cachara.Content.API
{
    public partial class CacharaContentService<TOptions> where TOptions : CacharaContentOptions, new()
    {
        private IConfiguration Configuration;

        private IHostEnvironment Environment;

        private TOptions Options { get; set; }
        
        public CacharaContentService(IHostEnvironment environment, IConfiguration configuration)
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
                Console.WriteLine($"Could not Bind Options for {nameof(CacharaContentService<TOptions>)}");
                throw;
            }
        }
        
        public void ConfigureServices(IServiceCollection services)
        {
            // Dependency Injection Options
            OptionsServiceCollectionExtensions.AddOptions<TOptions>(services).Bind(Configuration);

            services.AddScoped<IGeneralDataProtectionService, AesGeneralDataProtectionService>(p =>
                new AesGeneralDataProtectionService(Options.Security.Key));

            services.AddScoped<IPostService, PostService>();
            services.AddScoped<IPostManagerService, PostManagerService>();

            services.AddScoped<IPostRepository, PostRepository>();
            
            services.AddHealthChecks()
                .AddSqlServer(
                    connectionString: Configuration.GetConnectionString(Options.SqlDb),
                    healthQuery: "SELECT 1;",
                    name: "database_check",
                    failureStatus: HealthStatus.Degraded);
            
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
                    SchemaName = "CacharaContentHangfire",
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
            services.AddDbContext<CacharaContentDbContext>(options =>
            {
                options.UseSqlServer(Options.SqlDb);
                options.UseQueryTrackingBehavior((QueryTrackingBehavior.NoTracking));
                options.EnableSensitiveDataLogging(Environment.IsDevelopment());
            }).AddAsyncInitializer<DbContextInitializer<CacharaContentDbContext>>()
            .AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<CacharaContentDbContext>());;
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
                endpoints.MapHealthChecks("/health"); 
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

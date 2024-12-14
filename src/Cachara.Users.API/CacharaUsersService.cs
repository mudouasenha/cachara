using System.Reflection;
using System.Text.Json.Serialization;
using Cachara.Shared.Infrastructure.AzureServiceBus;
using Cachara.Shared.Infrastructure.Data.Interfaces;
using Cachara.Shared.Infrastructure.Hangfire;
using Cachara.Shared.Infrastructure.Security;
using Cachara.Users.API.API.Extensions;
using Cachara.Users.API.API.Hangfire;
using Cachara.Users.API.API.Options;
using Cachara.Users.API.API.Security;
using Cachara.Users.API.API.Swagger;
using Cachara.Users.API.Infrastructure.Data;
using Cachara.Users.API.Infrastructure.Data.Repository;
using Cachara.Users.API.Services;
using Cachara.Users.API.Services.Abstractions;
using Flurl;
using Hangfire;
using Hangfire.Console;
using Hangfire.SqlServer;
using Hellang.Middleware.ProblemDetails;
using Hellang.Middleware.ProblemDetails.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Cachara.Users.API
{
    public sealed class CacharaUsersService<TOptions> where TOptions : CacharaOptions, new()
    {
        private readonly IConfiguration _configuration;

        private readonly IHostEnvironment _environment;

        private TOptions Options { get; set; }
        
        public CacharaUsersService(IHostEnvironment environment, IConfiguration configuration)
        {
            _environment = environment;
            _configuration = configuration;
            Options = new TOptions()
            {
                Name = GetType().Name
            };
            try
            {
                _configuration.Bind(Options);
            }
            catch (Exception)
            {
                Console.WriteLine($"Could not Bind Options for {nameof(CacharaUsersService<TOptions>)}");
                throw;
            }
        }
        
        public void ConfigureServices(IServiceCollection services)
        {
            // Dependency Injection Options
            services.AddOptions<TOptions>().Bind(_configuration);

            services.AddScoped<IGeneralDataProtectionService, AesGeneralDataProtectionService>(_ =>
                new AesGeneralDataProtectionService(Options.Security.Key));
            services.AddSingleton<IJwtProvider, JwtProvider>(_ => new(Options.Jwt));
            
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IUserProfileService, UserProfileService>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<UserSubscriptionProvider>();
            services.AddTransient<IClaimsTransformation, CustomClaimsTransformation>();
            
            services.AddHealthChecks()
                .AddSqlServer(
                    connectionString: Options.SqlDb,
                    healthQuery: "SELECT 1;",
                    name: "database_check",
                    failureStatus: HealthStatus.Degraded);

            services.AddAuthorization(options =>
            {
                options.AddPolicy("management-user", policy => policy
                    .RequireAuthenticatedUser()
                    .RequireClaim(
                        CustomClaims.Subscription,
                        allowedValues: [Subscription.Management.ToString()]
                        ));

                options.AddPolicy("premium-user", policy => policy
                    .RequireAuthenticatedUser()
                    .RequireClaim(
                        CustomClaims.Subscription,
                        allowedValues: [Subscription.Premium.ToString(), Subscription.Management.ToString()]
                    ));

                options.AddPolicy("standard-user", policy => policy
                    .RequireAuthenticatedUser()
                    .RequireClaim(
                        CustomClaims.Subscription,
                        allowedValues:
                        [
                            Subscription.Management.ToString(), Subscription.Premium.ToString(),
                            Subscription.Standard.ToString()
                        ]
                    ));
            });
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer();
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            
            services.AddProblemDetails(delegate (Hellang.Middleware.ProblemDetails.ProblemDetailsOptions _) { });
            
            services.AddControllers(options =>
            {
                options.Conventions.Add(new ApiExplorerGroupPerVersionConvention());

                options.InputFormatters.Add(new TextPlainInputFormatter());
                options.InputFormatters.Add(new StreamInputFormatter());
            }).AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.Never;
            })
            .AddProblemDetailsConventions();

            services.AddOpenApiDocument();
            services.AddResponseCaching();
            services.AddEndpointsApiExplorer();
            services.AddCustomSwagger();

            services.AddSingleton<IServiceBusQueue, ServiceBusQueue>(
                _ => new ServiceBusQueue(Options.CacharaContent.ServiceBusConn)
            );
            
            ConfigureHangfire(services);
            ConfigureDataAccess(services);
        }

        private void ConfigureHangfire(IServiceCollection services)
        {
            services.AddScoped<IBackgroundServiceManager, BackgroundServiceManager>();

            services.AddHangfire((_, config) =>
            {
                config.UseSimpleAssemblyNameTypeSerializer();
                config.SetDataCompatibilityLevel(CompatibilityLevel.Version_180);
                config.UseRecommendedSerializerSettings(_ =>
                {
                    //x.Converters.Add(new JsonDateOnlyConverter());
                });
                config.UseSqlServerStorage(Options.JobsSqlDb, new SqlServerStorageOptions()
                {
                    SchemaName = "CacharaUsersHangfire",
                    PrepareSchemaIfNecessary = true
                });
                config.UseConsole();
            });
            
            int totalWorkerCount = Environment.ProcessorCount * 20;
            services.AddHangfireServer(options =>
            {
                options.WorkerCount = totalWorkerCount;
                options.Queues = new string[]
                {
                    "default"
                };
            });
        }

        private void ConfigureDataAccess(IServiceCollection services)
        {
            services.AddDbContext<CacharaUsersDbContext>(options =>
            {
                options.UseSqlServer(Options.SqlDb);
                options.UseQueryTrackingBehavior((QueryTrackingBehavior.NoTracking));
                options.EnableSensitiveDataLogging(_environment.IsDevelopment());
            }).AddAsyncInitializer<DbContextInitializer<CacharaUsersDbContext>>()
            .AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<CacharaUsersDbContext>());
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

                if (swaggerGenOptions == null) return;
                
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
        
        public void Configure(IApplicationBuilder app)
        {
            ConfigureApp(app);
        }
    }
}

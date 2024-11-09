using System.Reflection;
using System.Text.Json.Serialization;
using Cachara.API.Hangfire;
using Cachara.Users.API.Extensions;
using Cachara.Data.EF;
using Cachara.Data.Interfaces;
using Cachara.Data.Persistence.Connections;
using Cachara.Domain.Abstractions.Security;
using Cachara.Domain.Interfaces.Services;
using Cachara.Services.Security;
using Cachara.Services.Services;
using Cachara.Users.API.API.Security;
using Cachara.Users.API.Controllers.Public;
using Cachara.Users.API.Infrastructure;
using Cachara.Users.API.Infrastructure.Data.Repository;
using Cachara.Users.API.Options;
using Cachara.Users.API.Services;
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
    public partial class CacharaUsersService<TOptions> where TOptions : CacharaOptions, new()
    {
        private IConfiguration Configuration;

        private IHostEnvironment Environment;

        private TOptions Options { get; set; }
        
        public CacharaUsersService(IHostEnvironment environment, IConfiguration configuration)
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
                Console.WriteLine($"Could not Bind Options for {nameof(CacharaUsersService<TOptions>)}");
                throw;
            }
        }
        
        public void ConfigureServices(IServiceCollection services)
        {
            // Dependency Injection Options
            OptionsServiceCollectionExtensions.AddOptions<TOptions>(services).Bind(Configuration);

            services.AddScoped<IGeneralDataProtectionService, AesGeneralDataProtectionService>(p =>
                new AesGeneralDataProtectionService(Options.Security.Key));
            
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IUserProfileService, UserProfileService>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<UserSubscriptionProvider>();
            services.AddTransient<IClaimsTransformation, CustomClaimsTransformation>();
            
            services.AddHealthChecks()
                .AddSqlServer(
                    connectionString: Configuration.GetConnectionString(Options.SqlDb),
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
                
                options.AddPolicy("standard-user", policy => policy
                    .RequireAuthenticatedUser()
                    .RequireClaim(
                        CustomClaims.Subscription,
                        allowedValues: [Subscription.Management.ToString(), Subscription.Standard.ToString()]
                    ));
            });
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer();
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
                    SchemaName = "CacharaUsersHangfire",
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
            services.AddDbContext<CacharaUsersDbContext>(options =>
            {
                options.UseSqlServer(Options.SqlDb);
                options.UseQueryTrackingBehavior((QueryTrackingBehavior.NoTracking));
                options.EnableSensitiveDataLogging(Environment.IsDevelopment());
            }).AddAsyncInitializer<DbContextInitializer<CacharaUsersDbContext>>()
            .AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<CacharaUsersDbContext>());;
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

using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;
using Cachara.Shared.Infrastructure;
using Cachara.Shared.Infrastructure.AzureServiceBus;
using Cachara.Shared.Infrastructure.Data.Interfaces;
using Cachara.Shared.Infrastructure.Hangfire;
using Cachara.Shared.Infrastructure.Middlewares;
using Cachara.Shared.Infrastructure.Security;
using Cachara.Users.API.API.Authentication;
using Cachara.Users.API.API.Hangfire;
using Cachara.Users.API.API.Options;
using Cachara.Users.API.API.Security;
using Cachara.Users.API.API.Swagger;
using Cachara.Users.API.Controllers;
using Cachara.Users.API.Infrastructure.Cache;
using Cachara.Users.API.Infrastructure.Data;
using Cachara.Users.API.Infrastructure.Data.Repository;
using Cachara.Users.API.Infrastructure.SessionManagement;
using Cachara.Users.API.Services;
using Cachara.Users.API.Services.Abstractions;
using Hangfire;
using Hangfire.Console;
using Hangfire.PostgreSql;
using Hangfire.SqlServer;
using HealthChecks.UI.Client;
using Hellang.Middleware.ProblemDetails;
using Hellang.Middleware.ProblemDetails.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using NSwag;
using NSwag.Generation.Processors.Security;
using Scalar.AspNetCore;
using ProblemDetailsOptions = Hellang.Middleware.ProblemDetails.ProblemDetailsOptions;
using Subscription = Cachara.Users.API.API.Security.Subscription;

namespace Cachara.Users.API;

public sealed class CacharaUsersService<TOptions> where TOptions : CacharaOptions, new()
{
    private readonly IConfiguration _configuration;

    private readonly IHostEnvironment _environment;

    public CacharaUsersService(IHostEnvironment environment, IConfiguration configuration)
    {
        _environment = environment;
        _configuration = configuration;
        Options = new TOptions { Name = GetType().Name };
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

    private TOptions Options { get; }

    public void Configure(IApplicationBuilder app)
    {
        ConfigureApp(app);
    }

    public void ConfigureServices(IServiceCollection services)
    {
        // Dependency Injection Options
        services.AddOptions<TOptions>().Bind(_configuration);

        AddServicesAndRepositories(services);
        ConfigureAzure(services);

        AddHealthChecks(services);

        AddSecurity(services);

        services.AddAutoMapper(Assembly.GetExecutingAssembly());

        ConfigureEndpoints(services);

        services.AddMemoryCache();
        services.AddStackExchangeRedisCache(options => { options.Configuration = Options.RedisConnection; });
        //services.AddDistributedMemoryCache() IMPORTANTE PARA USAR A INTERFACE IDISTRIBUTEDCACHE, MAS SEM PRECISAR
        // IMPLEMENTAR O REDIS

#pragma warning disable EXTEXP0018
        services.AddHybridCache(options =>
        {
            options.MaximumPayloadBytes = 1024 * 1024;
            options.MaximumKeyLength = 1024;
            options.DefaultEntryOptions = new HybridCacheEntryOptions
            {
                Expiration = TimeSpan.FromSeconds(30), LocalCacheExpiration = TimeSpan.FromSeconds(10)
            };
        });
#pragma warning restore EXTEXP0018

        ConfigureHangfire(services);
        ConfigureDataAccess(services);
    }

    private static void ConfigureEndpoints(IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        services.AddProblemDetails(delegate(ProblemDetailsOptions _) { });

        services.AddControllers(options =>
            {
                options.InputFormatters.Add(new TextPlainInputFormatter());
                options.InputFormatters.Add(new StreamInputFormatter());
            }).AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.Never;
            })
            .AddProblemDetailsConventions();

        services.AddEndpointsApiExplorer();

        services.AddOpenApiDocument(options =>
            {
                options.DocumentName = "management";
                options.PostProcess = document =>
                {
                    document.DocumentPath = "/openapi/{documentName}.json";
                    document.Info = new OpenApiInfo
                    {
                        Title = "Cachara Users API - Management API",
                        Version = "1.2024.12.1",
                        Description = "This API contains all endpoints for users operations."
                    };

                    document.Info.Contact = new OpenApiContact
                    {
                        Email = "support@cachara.test",
                        Name = "Cachara Support",
                        Url = "https://github.com/mudouasenha/cachara"
                    };
                };
                options.ApiGroupNames = new[] { "management" };
            });

        services.AddOpenApiDocument(options =>
            {
                options.DocumentName = "public";
                options.AddSecurity("Bearer", new OpenApiSecurityScheme
                {
                    Type = OpenApiSecuritySchemeType.Http,
                    Scheme = JwtBearerDefaults.AuthenticationScheme,
                    BearerFormat = "JWT",
                    In = OpenApiSecurityApiKeyLocation.Header,
                    Description = "Enter 'Bearer {token}' in the Authorization header."
                });
                options.OperationProcessors.Add(new AspNetCoreOperationSecurityScopeProcessor(JwtBearerDefaults.AuthenticationScheme));

                options.PostProcess = document =>
                {
                    document.DocumentPath = "/openapi/{documentName}.json";
                    document.Info = new OpenApiInfo
                    {
                        Title = "Cachara Users API - Public API",
                        Version = "1.2024.12.1",
                        Description = "This API contains all endpoints for users operations."
                    };

                    document.Info.Contact = new OpenApiContact
                    {
                        Email = "support@cachara.test",
                        Name = "Cachara Support",
                        Url = "https://github.com/mudouasenha/cachara"
                    };
                };
                options.ApiGroupNames = new[] { "public" };
            });

        services.AddOpenApiDocument(options =>
            {
                options.DocumentName = "internal";
                options.PostProcess = document =>
                {
                    document.DocumentPath = "/openapi/{documentName}.json";
                    document.Info = new OpenApiInfo
                    {
                        Title = "Cachara Users API - Internal API",
                        Version = "1.2024.12.1",
                        Description = "This API contains all endpoints for users operations."
                    };

                    document.Info.Contact = new OpenApiContact
                    {
                        Email = "support@cachara.test",
                        Name = "Cachara Support",
                        Url = "https://github.com/mudouasenha/cachara"
                    };
                };
                options.ApiGroupNames = new[] { "internal" };
            });

        services.AddResponseCaching();
    }

    private void ConfigureAzure(IServiceCollection services)
    {
        services.AddSingleton<IServiceBusQueue, ServiceBusQueue>(
            _ => new ServiceBusQueue(Options.CacharaContent.ServiceBusConn)
        );
    }

    private void AddSecurity(IServiceCollection services)
    {
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(opts =>
            {
                opts.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateAudience = true,
                    ValidateIssuer = true,
                    ValidateIssuerSigningKey = true,
                    ValidAudiences = Options.Jwt.Audiences,
                    ValidIssuers = Options.Jwt.Issuers,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Options.Jwt.Key))
                };
            });

        // TODO: Add requirement of "user must own document in order to access"
        // TODO: Create Minimum Age requirement.
        services.AddAuthorization(options =>
        {
            options.AddPolicy(Policies.ManagementUser, policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireClaim(
                        CustomClaims.Subscription,
                        Subscription.Management.ToString()
                    );
            });

            options.AddPolicy(Policies.PremiumUser, policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireClaim(
                        CustomClaims.Subscription,
                        Subscription.Premium.ToString(), Subscription.Management.ToString()
                    );
            });

            options.AddPolicy(Policies.StandardUser, policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireClaim(
                        CustomClaims.Subscription,
                            Subscription.Management.ToString(), Subscription.Premium.ToString(),
                            Subscription.Standard.ToString()
                    );
            });
        });
    }

    private void AddHealthChecks(IServiceCollection services)
    {
        services.AddHealthChecks()
            .AddSqlServer(
                Options.SqlDb,
                "SELECT 1;",
                name: "Sql Server",
                tags: new[] { "relational", "database" },
                failureStatus: HealthStatus.Degraded)
            .AddRedis(Options.RedisConnection, name: "Redis", tags: new[] { "cache", "database" },
                failureStatus: HealthStatus.Degraded);
    }

    private void AddServicesAndRepositories(IServiceCollection services)
    {
        services.AddScoped<IGeneralDataProtectionService, AesGeneralDataProtectionService>(_ =>
            new AesGeneralDataProtectionService(Options.Security.Key));
        services.AddSingleton<IJwtProvider, JwtProvider>(_ => new JwtProvider(Options.Jwt));

        services.AddScoped<IUserService, UserService>();
        services.AddScoped<UserAuthenticationService>();
        services.AddScoped<IUserProfileService, UserProfileService>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<UserSubscriptionProvider>();
        services.AddTransient<IClaimsTransformation, CustomClaimsTransformation>();

        services.AddScoped<ICacheService, CacheService>();
        services.AddSingleton<IAccountService<UserAccount>, UserAccountService>();
        services.AddScoped<ISessionStoreService<UserAccount>, SessionStoreService>();
    }

    private void ConfigureHangfire(IServiceCollection services)
    {
        services.AddScoped<IBackgroundServiceManager, BackgroundServiceManager>();

        services.AddHangfire((_, config) =>
        {
            config.UseSimpleAssemblyNameTypeSerializer();
            config.SetDataCompatibilityLevel(CompatibilityLevel.Version_180);
            config.UseRecommendedSerializerSettings();
            config.UsePostgreSqlStorage(p =>
                {
                    p.UseNpgsqlConnection(Options.JobsSqlDb);
                },
                new PostgreSqlStorageOptions() { SchemaName = "UsersHangfire", PrepareSchemaIfNecessary = true });
            config.UseConsole();
        });

        var totalWorkerCount = Environment.ProcessorCount * 20;
        services.AddHangfireServer(options =>
        {
            options.WorkerCount = totalWorkerCount;
            options.Queues = new[] { "default" };
        });
    }

    private void ConfigureDataAccess(IServiceCollection services)
    {
        services.AddDbContext<CacharaUsersDbContext>(options =>
            {
                options.UseNpgsql(Options.SqlDb);
                options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
                options.EnableSensitiveDataLogging(_environment.IsDevelopment());
            }).AddAsyncInitializer<DbContextInitializer<CacharaUsersDbContext>>()
            .AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<CacharaUsersDbContext>());
    }

    public void ConfigureApp(IApplicationBuilder app)
    {
        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();

            app.UseOpenApi(options =>
            {
                options.Path = "/openapi/{documentName}.json";
                options.PostProcess = (document, request) =>
                {
                    document.BasePath = "";
                    if (request.Headers.TryGetValue("X-Forwarded-Host", out var hosts))
                    {
                        document.Host = hosts.FirstOrDefault();
                    }
                };
            });
            endpoints.MapScalarApiReference("scalar", options =>
            {
                options
                    .WithPreferredScheme("Bearer")
                    .WithHttpBearerAuthentication(bearer =>
                    {
                        bearer.Token = "your-bearer-token";
                    });

                options.AddDocument("internal");
                options.AddDocument("management");
                options.AddDocument("public");
            });

            endpoints.MapHealthChecks("/health", new HealthCheckOptions()
            {
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });
        });

        app.UseMiddleware<RequestTracingMiddleware>();

        app.UseHangfireDashboard();
    }
}

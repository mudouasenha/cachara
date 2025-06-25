using System.Text;
using System.Text.Json.Serialization;
using Cachara.Shared.Application;
using Cachara.Shared.Infrastructure;
using Cachara.Shared.Infrastructure.AzureServiceBus;
using Cachara.Shared.Infrastructure.Data.Interfaces;
using Cachara.Shared.Infrastructure.Hangfire;
using Cachara.Shared.Infrastructure.Middlewares;
using Cachara.Shared.Infrastructure.Security;
using Cachara.Users.API.API.Authentication;
using Cachara.Users.API.API.Options;
using Cachara.Users.API.API.Security;
using Cachara.Users.API.API.Swagger;
using Cachara.Users.API.Infrastructure;
using Cachara.Users.API.Infrastructure.Cache;
using Cachara.Users.API.Infrastructure.Data;
using Cachara.Users.API.Infrastructure.Data.Repository;
using Cachara.Users.API.Infrastructure.SessionManagement;
using Cachara.Users.API.Services.Abstractions;
using Cachara.Users.API.Services.Externals;
using Cachara.Users.API.Services.Mappings;
using Hangfire;
using Hangfire.Console;
using Hangfire.PostgreSql;
using HealthChecks.UI.Client;
using Hellang.Middleware.ProblemDetails;
using Hellang.Middleware.ProblemDetails.Mvc;
using Mapster;
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

public sealed class CacharaUsersService(IHostEnvironment environment, IConfiguration configuration)
    : CacharaService<CacharaUserOptions>(environment, configuration)
{
    private readonly IHostEnvironment _environment = environment;

    public override void ConfigureServices(IServiceCollection services)
    {
        base.ConfigureServices(services);
        AddServices(services);
        ConfigureInfrastructure(services);
        ConfigureEndpoints(services);
        // Dependency Injection Options
        //services.AddOptions<CacharaUserOptions>().Bind(_configuration);


    }

    private void ConfigureInfrastructure(IServiceCollection services)
    {
        ConfigureExternalServices(services);
        AddHealthChecks(services);
        AddSecurity(services);
        ConfigureHangfire(services);

        services.AddStackExchangeRedisCache(options => { options.Configuration = Options.RedisConnection; });
        services.AddDistributedMemoryCache();

        services.AddHybridCache(options =>
        {
            options.MaximumPayloadBytes = 1024 * 1024;
            options.MaximumKeyLength = 1024;
            options.DefaultEntryOptions = new HybridCacheEntryOptions
            {
                Expiration = TimeSpan.FromSeconds(30), LocalCacheExpiration = TimeSpan.FromSeconds(10)
            };
        });

        services.AddDbContext<CacharaUsersDbContext>(options =>
            {
                options.UseNpgsql(Options.SqlDb);
                options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
                options.EnableSensitiveDataLogging(_environment.IsDevelopment());
            }).AddAsyncInitializer<DbContextInitializer<CacharaUsersDbContext>>()
            .AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<CacharaUsersDbContext>());

        services.AddScoped<IUserRepository, UserRepository>();
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

    private void ConfigureExternalServices(IServiceCollection services)
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
            .AddNpgSql(
                Options.SqlDb,
                "SELECT 1;",
                name: "PostgreSQL",
                tags: new[] { "relational", "database" },
                failureStatus: HealthStatus.Degraded)
            .AddRedis(
                Options.RedisConnection,
                name: "Redis",
                tags: new[] { "cache", "database" },
                failureStatus: HealthStatus.Degraded)
            .AddKafka(
                setup =>
                {
                    setup.BootstrapServers = Options.KafkaConnection;
                },
                name: "kafka",
                tags: new[] { "messaging", "event-streaming", "distributed", "integration" },
                failureStatus: HealthStatus.Degraded);

    }

    private void AddServices(IServiceCollection services)
    {
        services.AddMapster();
        UsersMappings.Configure();

        services.AddScoped<IGeneralDataProtectionService, AesGeneralDataProtectionService>(_ =>
            new AesGeneralDataProtectionService(Options.Security.Key));
        services.AddSingleton<IJwtProvider, JwtProvider>(_ => new JwtProvider(Options.Jwt));

        services.AddScoped<IUserService, UserService>();
        services.AddScoped<UserAuthenticationService>();
        services.AddScoped<IUserProfileService, UserProfileService>();
        services.AddScoped<UserSubscriptionProvider>();
        services.AddTransient<IClaimsTransformation, CustomClaimsTransformation>();

        services.AddScoped<ICacheService, CacheService>();
        services.AddScoped<IAccountService<UserAccount>, UserAccountService>();
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

        services.AddHangfireServer(options =>
        {
            options.WorkerCount = 5;
            options.Queues = new[] { "default" };
        });
    }

    protected override void ConfigureApp(IApplicationBuilder app)
    {
        base.ConfigureApp(app);

        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();

        app.UseMiddleware<SessionValidationMiddleware>();

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
                    .AddPreferredSecuritySchemes("Bearer")
                    .AddHttpAuthentication("Bearer",config =>
                    {
                        config.Token = "your-bearer-token";
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

        app.UseHangfireDashboard();
    }

}

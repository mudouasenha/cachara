using System.Text.Json.Serialization;
using Cachara.Content.API.API.Options;
using Cachara.Content.API.Infrastructure;
using Cachara.Content.API.Infrastructure.Clients;
using Cachara.Content.API.Infrastructure.Data;
using Cachara.Content.API.Infrastructure.Data.Repository;
using Cachara.Content.API.Services;
using Cachara.Content.API.Services.External;
using Cachara.Shared.Application;
using Cachara.Shared.Application.Mvc.Formatters;
using Cachara.Shared.Application.Services;
using Cachara.Shared.Infrastructure;
using Cachara.Shared.Infrastructure.Data.EF;
using Cachara.Shared.Infrastructure.Data.Interfaces;
using Cachara.Shared.Infrastructure.Hangfire;
using Cachara.Shared.Infrastructure.Middlewares;
using Hangfire;
using Hangfire.Console;
using Hangfire.PostgreSql;
using HealthChecks.UI.Client;
using Hellang.Middleware.ProblemDetails;
using Hellang.Middleware.ProblemDetails.Mvc;
using Mapster;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NSwag;
using Scalar.AspNetCore;
using ProblemDetailsOptions = Hellang.Middleware.ProblemDetails.ProblemDetailsOptions;
namespace Cachara.Content.API;

public sealed class CacharaContentService(IHostEnvironment environment, IConfiguration configuration)
    : CacharaService<CacharaContentOptions>(environment, configuration)
{
    public override void ConfigureServices(IServiceCollection services)
    {
        base.ConfigureServices(services);
        AddServices(services);
        ConfigureInfrastructure(services);
        ConfigureEndpoints(services);
    }

    private void ConfigureInfrastructure(IServiceCollection services)
    {
        ConfigureExternalServices(services);
        AddHealthChecks(services);
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
                        Title = "Cachara Posts API - Management API",
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

                options.PostProcess = document =>
                {
                    document.DocumentPath = "/openapi/{documentName}.json";
                    document.Info = new OpenApiInfo
                    {
                        Title = "Cachara Posts API - Public API",
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


    private static void ConfigureExternalServices(IServiceCollection services)
    {
        services.AddHttpClient<GitHubService>((serviceProvider, client) =>
        {
            // client.DefaultRequestHeaders.Add("Authorization", Options.GitHubToken);
            // client.DefaultRequestHeaders.Add("User-Agent", Options.UserAgent);
            // client.BaseAddress = new Uri("");
        });

        // if (Options.CacharaUsers?.ListenerEnabled == true)
        // {
        //     services.AddHostedService<UserListernerService>();
        // }

        // services.AddAzureClients(builder =>
        // {
        //     if (Options.CacharaUsers?.ListenerEnabled == true)
        //     {
        //         builder.AddServiceBusClient(Options.CacharaUsers.ServiceBusConn)
        //             .WithName(UserListernerService.UsersServiceBusKey);
        //     }
        // });
    }

    private static void AddSecurity(IServiceCollection services)
    {
        // TODO: Implement security between services
    }

    private void ConfigureDataAccess(IServiceCollection services)
    {
        services.AddScoped<IPostRepository, PostRepository>();

        services.AddDbContext<CacharaContentDbContext>(options =>
            {
                options.UseSqlServer(Options.SqlDb);
                options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
                options.EnableSensitiveDataLogging(Environment.IsDevelopment());
            }).AddAsyncInitializer<DbContextInitializer<CacharaContentDbContext>>()
            .AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<CacharaContentDbContext>());
        ;
    }

    private void AddHealthChecks(IServiceCollection services)
    {
        services.AddHealthChecks()
            .AddNpgSql(
                Options.SqlDb,
                "SELECT 1;",
                name: "PostgreSQL",
                tags: new[] { "relational", "database" },
                failureStatus: HealthStatus.Degraded);

    }

    private static void AddServices(IServiceCollection services)
    {
        services.AddMapster();
        //UsersMappings.Configure();
        services.AddScoped<PostService>();
        services.AddScoped<ManagementPostService>();
        services.AddScoped<PublicPostService>();
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

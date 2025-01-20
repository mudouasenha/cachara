using System.Reflection;
using System.Text.Json.Serialization;
using Cachara.Content.API.API.BackgroundServices;
using Cachara.Content.API.API.Extensions;
using Cachara.Content.API.API.Hangfire;
using Cachara.Content.API.API.Options;
using Cachara.Content.API.Infrastructure;
using Cachara.Content.API.Infrastructure.Clients;
using Cachara.Content.API.Infrastructure.Data;
using Cachara.Content.API.Infrastructure.Data.Repository;
using Cachara.Content.API.Services;
using Cachara.Content.API.Services.External;
using Cachara.Content.API.Services.Internal;
using Cachara.Shared.Infrastructure.AzureServiceBus;
using Cachara.Shared.Infrastructure.Data.Interfaces;
using Cachara.Shared.Infrastructure.Hangfire;
using Cachara.Shared.Infrastructure.Security;
using Flurl;
using Hangfire;
using Hangfire.Console;
using Hangfire.SqlServer;
using Hellang.Middleware.ProblemDetails;
using Hellang.Middleware.ProblemDetails.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.OpenApi.Models;
using Scalar.AspNetCore;
using Swashbuckle.AspNetCore.SwaggerGen;
using ProblemDetailsOptions = Hellang.Middleware.ProblemDetails.ProblemDetailsOptions;

namespace Cachara.Content.API;

public class CacharaContentService<TOptions> where TOptions : CacharaContentOptions, new()
{
    private readonly IConfiguration Configuration;

    private readonly IHostEnvironment Environment;

    public CacharaContentService(IHostEnvironment environment, IConfiguration configuration)
    {
        Environment = environment;
        Configuration = configuration;
        Options = new TOptions { Name = GetType().Name };
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

    private TOptions Options { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        // Dependency Injection Options
        services.AddOptions<TOptions>().Bind(Configuration);

        services.AddScoped<IGeneralDataProtectionService, AesGeneralDataProtectionService>(p =>
            new AesGeneralDataProtectionService(Options.Security.Key));

        services.AddScoped<IPostService, PostService>();
        services.AddScoped<IPostManagerService, PostManagerService>();

        services.AddScoped<IPostRepository, PostRepository>();

        services.AddHealthChecks()
            .AddSqlServer(
                Configuration.GetConnectionString(Options.SqlDb),
                "SELECT 1;",
                name: "database_check",
                failureStatus: HealthStatus.Degraded);

        services.AddAutoMapper(Assembly.GetExecutingAssembly());

        services.AddProblemDetails(delegate(ProblemDetailsOptions opts) { });

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

        services.AddOpenApi(opt =>
        {
            opt.AddDocumentTransformer((document, context, cancellationToken) =>
            {
                document.Info = new OpenApiInfo
                {
                    Title = "Cachara Content API",
                    Version = "1.2024.12.1",
                    Description = "This API contains all endpoints for Users Content operations."
                };

                document.Info.Contact = new OpenApiContact
                {
                    Email = "support@cachara.test",
                    Name = "Cachara Support",
                    Url = new Uri("https://github.com/mudouasenha/cachara")
                };

                return Task.CompletedTask;
            });
        });
        services.AddResponseCaching();
        services.AddEndpointsApiExplorer();
        services.AddCustomSwagger();

        services.AddSingleton<IServiceBusQueue, ServiceBusQueue>(
            x => new ServiceBusQueue(Options.CacharaUsers.ServiceBusConn ?? "")
        );

        ConfigureExternalServices(services);

        if (Options.CacharaUsers?.ListenerEnabled == true)
        {
            services.AddHostedService<UserListernerService>();
        }

        ConfigureHangfire(services);
        ConfigureDataAccess(services);
        ConfigureHttpClients(services);
    }

    private void ConfigureHttpClients(IServiceCollection services)
    {
        services.AddHttpClient<GitHubService>((serviceProvider, client) =>
        {
            // client.DefaultRequestHeaders.Add("Authorization", Options.GitHubToken);
            // client.DefaultRequestHeaders.Add("User-Agent", Options.UserAgent);
            // client.BaseAddress = new Uri("");
        });
    }

    private void ConfigureExternalServices(IServiceCollection services)
    {
        services.AddAzureClients(builder =>
        {
            if (Options.CacharaUsers?.ListenerEnabled == true)
            {
                builder.AddServiceBusClient(Options.CacharaUsers.ServiceBusConn)
                    .WithName(UserListernerService.UsersServiceBusKey);
            }
        });
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
            config.UseSqlServerStorage(Options.JobsSqlDb,
                new SqlServerStorageOptions { SchemaName = "CacharaContentHangfire", PrepareSchemaIfNecessary = true });
            config.UseConsole();
        });

        var totalWorkerCount = System.Environment.ProcessorCount * 20;
        services.AddHangfireServer(options =>
        {
            options.WorkerCount = totalWorkerCount;
            options.Queues = new[] { "default" };
        });
    }

    public void ConfigureDataAccess(IServiceCollection services)
    {
        services.AddDbContext<CacharaContentDbContext>(options =>
            {
                options.UseSqlServer(Options.SqlDb);
                options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
                options.EnableSensitiveDataLogging(Environment.IsDevelopment());
            }).AddAsyncInitializer<DbContextInitializer<CacharaContentDbContext>>()
            .AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<CacharaContentDbContext>());
        ;
    }

    public void ConfigureApp(IApplicationBuilder app)
    {
        app.UseProblemDetails();
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
            endpoints.MapScalarApiReference();
            endpoints.MapControllers();
            endpoints.MapHealthChecks("/health");
            endpoints.MapSwagger();
            endpoints.MapOpenApi();
        });


        app.UseHangfireDashboard();
    }

    public virtual void Configure(IApplicationBuilder app)
    {
        ConfigureApp(app);
    }
}

using System.Reflection;
using System.Text.Json.Serialization;
using Cachara.Content.API.API.Options;
using Cachara.Content.API.Infrastructure;
using Cachara.Content.API.Infrastructure.Clients;
using Cachara.Content.API.Infrastructure.Data;
using Cachara.Content.API.Infrastructure.Data.Repository;
using Cachara.Content.API.Services;
using Cachara.Content.API.Services.External;
using Cachara.Content.API.Services.Internal;
using Cachara.Shared.Application;
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
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.OpenApi.Models;
using Scalar.AspNetCore;
using Swashbuckle.AspNetCore.SwaggerGen;
using ProblemDetailsOptions = Hellang.Middleware.ProblemDetails.ProblemDetailsOptions;
using StreamInputFormatter = Cachara.Content.API.Infrastructure.StreamInputFormatter;

namespace Cachara.Content.API;

public sealed class CacharaContentService(IHostEnvironment environment, IConfiguration configuration)
    : CacharaService<CacharaContentOptions>(environment, configuration)
{
    public override void ConfigureServices(IServiceCollection services)
    {
        base.ConfigureServices(services);

        services.AddScoped<IGeneralDataProtectionService, AesGeneralDataProtectionService>(p =>
            new AesGeneralDataProtectionService(Options.Security.Key));

        services.AddScoped<IPostService, PostService>();
        services.AddScoped<IPostManagerService, PostManagerService>();



        services.AddHealthChecks()
            .AddSqlServer(
                Configuration.GetConnectionString(Options.SqlDb),
                "SELECT 1;",
                name: "database_check",
                failureStatus: HealthStatus.Degraded);

        services.AddMapster();

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
            // TODO: Split/group controllers https://stackoverflow.com/questions/79220015/how-to-group-controller-using-openapi-and-scalar-api-documentations
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

        services.AddSingleton<IServiceBusQueue, ServiceBusQueue>(
            x => new ServiceBusQueue(Options.CacharaUsers.ServiceBusConn ?? "")
        );

        ConfigureExternalServices(services);



    }

    private void ConfigureInfrastructure(IServiceCollection services)
    {
        ConfigureExternalServices(services);
        ConfigureHangfire(services);
        ConfigureDataAccess(services);

        services.AddHttpClient<GitHubService>((serviceProvider, client) =>
        {
            // client.DefaultRequestHeaders.Add("Authorization", Options.GitHubToken);
            // client.DefaultRequestHeaders.Add("User-Agent", Options.UserAgent);
            // client.BaseAddress = new Uri("");
        });
    }

    private static void ConfigureExternalServices(IServiceCollection services)
    {
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

    private void ConfigureHangfire(IServiceCollection services)
    {
        services.AddScoped<IBackgroundServiceManager, BackgroundServiceManager>();

        services.AddHangfire((provider, config) =>
        {
            config.UseSimpleAssemblyNameTypeSerializer();
            config.SetDataCompatibilityLevel(CompatibilityLevel.Version_180);
            config.UseRecommendedSerializerSettings();
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

    protected override void ConfigureApp(IApplicationBuilder app)
    {
        base.ConfigureApp(app);

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

}

using Cachara.Users.API.API.Options;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using Serilog.Sinks.OpenTelemetry;
using OpenTelemetry.Instrumentation.AspNetCore;
using OpenTelemetry.Instrumentation.Http;

namespace Cachara.Users.API.API.Extensions;


public class CacharaLogging<TOptions> where TOptions : CacharaOptions, new()
{
    private readonly IConfiguration Configuration;

    private IHostEnvironment Environment;

    public CacharaLogging(IHostEnvironment environment, IConfiguration configuration)
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
            Console.WriteLine($"Could not Bind Options for {nameof(CacharaUsersService<TOptions>)}");
            throw;
        }
    }

    private TOptions Options { get; }
}

public static class LoggingExtensions
{
    public static ILoggingBuilder ConfigureSerilog(this ILoggingBuilder builder, IHostEnvironment environment,
        IConfiguration configuration)
    {
        OpenTelemetryOptions openTelemetryOptions = new();
        configuration.GetSection(OpenTelemetryOptions.Name).Bind(openTelemetryOptions);

        Log.Logger = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .WriteTo.Seq(
                serverUrl: openTelemetryOptions.Otlp.Endpoint,
                apiKey: openTelemetryOptions.Otlp.ApiKey)
            .CreateLogger();

        return builder.AddSerilog();
    }

    public static ILoggingBuilder ConfigureOpenTelemetry(this ILoggingBuilder builder, IHostEnvironment environment,
        IConfiguration configuration)
    {
        OpenTelemetryOptions openTelemetryOptions = new();
        configuration.GetSection(OpenTelemetryOptions.Name).Bind(openTelemetryOptions);

        builder.Services.AddOpenTelemetry()
            .WithTracing(tracing =>
            {
                tracing
                    .SetResourceBuilder(ResourceBuilder.CreateDefault()
                        .AddService("CacharaService")
                        .AddAttributes(new Dictionary<string, object>
                        {
                            ["deployment.environment"] = environment.EnvironmentName
                        }))
                    .AddAspNetCoreInstrumentation(options =>
                    {
                        options.RecordException = true;
                        options.EnrichWithHttpRequest = (activity, httpRequest) =>
                        {
                            activity.SetTag("http.method", httpRequest.Method);
                            activity.SetTag("http.url", httpRequest.Path.ToUriComponent());
                            activity.SetTag("user.id", httpRequest.HttpContext.User?.Identity?.Name ?? "anonymous");
                        };
                        options.EnrichWithHttpResponse = (activity, httpResponse) =>
                        {
                            activity.SetTag("http.status_code", httpResponse.StatusCode);
                        };
                        options.EnrichWithException = (activity, exception) =>
                        {
                            activity.SetTag("exception.message", exception.Message);
                        };
                    })
                    .AddHttpClientInstrumentation(options =>
                    {
                        options.RecordException = true;
                        options.EnrichWithHttpRequestMessage = (activity, request) =>
                        {
                            activity.SetTag("http.method", request.Method);
                            activity.SetTag("http.url", request.RequestUri);
                        };
                        options.EnrichWithHttpResponseMessage = (activity, response) =>
                        {
                            activity.SetTag("http.status_code", response.StatusCode);
                        };
                        options.EnrichWithException = (activity, exception) =>
                        {
                            activity.SetTag("exception.message", exception.Message);
                        };
                    })
                    .AddOtlpExporter(exporter =>
                    {
                        exporter.Endpoint = new Uri(openTelemetryOptions.Otlp.Endpoint);
                        exporter.Headers = $"X-Seq-ApiKey={openTelemetryOptions.Otlp.ApiKey}";
                    });
            });

        return builder;
    }
}


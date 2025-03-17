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
            .WriteTo.OpenTelemetry(x =>
            {
                x.Endpoint = openTelemetryOptions.Otlp.Endpoint;
                x.Protocol = OtlpProtocol.HttpProtobuf;
                x.Headers = new Dictionary<string, string> { ["X-Seq-ApiKey"] = openTelemetryOptions.Otlp.ApiKey };
                x.ResourceAttributes = new Dictionary<string, object> { ["service.name"] = "CacharaService" };
            })
            .CreateLogger();

        return builder;
    }

    public static ILoggingBuilder ConfigureOpenTelemetry(this ILoggingBuilder builder, IHostEnvironment environment,
        IConfiguration configuration)
{
    OpenTelemetryOptions openTelemetryOptions = new();
    configuration.GetSection(OpenTelemetryOptions.Name).Bind(openTelemetryOptions);

    builder.ClearProviders();
    builder.AddOpenTelemetry(x =>
    {
        x.IncludeScopes = true;
        x.IncludeFormattedMessage = true;

        x.AddConsoleExporter();
        x.AddOtlpExporter(exporter =>
        {
            exporter.Endpoint = new Uri(openTelemetryOptions.Otlp.Endpoint);
            exporter.Protocol = OtlpExportProtocol.HttpProtobuf;
            exporter.Headers = $"X-Seq-ApiKey={openTelemetryOptions.Otlp.ApiKey}";
        });
    });

    builder.Services.AddOpenTelemetry().WithTracing(tracing =>
    {
        tracing
            .SetResourceBuilder(ResourceBuilder.CreateDefault() // Correct resource setup
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
            });
    });

    return builder;
}

}

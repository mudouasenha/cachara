using Cachara.Content.API.API.Options;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Resources;
using Serilog;
using Serilog.Sinks.OpenTelemetry;

namespace Cachara.Content.API.API.Extensions;

public class CacharaLogging<TOptions> where TOptions : CacharaContentOptions, new()
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
            Console.WriteLine($"Could not Bind Options for {nameof(CacharaContentService<TOptions>)}");
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
                x.ResourceAttributes = new Dictionary<string, object> { ["service.name"] = "CacharaContentService" };
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
            x.SetResourceBuilder(ResourceBuilder.CreateEmpty()
                .AddService("CacharaContentService")
                .AddAttributes(new Dictionary<string, object>
                {
                    ["deployment.environment"] = environment.EnvironmentName
                }));

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

        return builder;
    }
}

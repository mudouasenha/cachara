using Cachara.API.Options;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;

namespace Cachara.API.Extensions;

public partial class CacharaLogging<TOptions> where TOptions : CacharaOptions, new()
{
    private IConfiguration Configuration;

    private IHostEnvironment Environment;

    private TOptions Options { get; set; }
        
    public CacharaLogging(IHostEnvironment environment, IConfiguration configuration)
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
            Console.WriteLine($"Could not Bind Options for {nameof(CacharaService<TOptions>)}");
            throw;
        }
    }
    
    
}

public static class LoggingExtensions
{
    public static ILoggingBuilder ConfigureOpenTelemetry(this ILoggingBuilder builder)
    {
        
        builder.ClearProviders();
        builder.AddOpenTelemetry(x =>
        {
            x.AddConsoleExporter();
            x.AddOtlpExporter(a =>
            {
                a.Endpoint = new Uri("http://localhost:5341/ingest/oltp/v1/logs"); // TODO: Add AppSettings
                a.Protocol = OtlpExportProtocol.HttpProtobuf;
                a.Headers = "X-Seq-ApiKey=ppcpJgDS0yiiLEc8j58h"; // TODO: Add AppSettings
            });
        });

        return builder;
    }
}
using Elastic.Apm.SerilogEnricher;
using Elasticsearch.Net;
using Microsoft.Extensions.Options;
using Serilog;
using Serilog.Events;
using Serilog.Extensions.Hosting;
using Serilog.Sinks.Elasticsearch;

namespace Cachara.Users.API.API.Extensions;

public class SerilogOptions
{
    public bool APMIntegrationEnabled { get; set; } = true;


    public SerilogWriteToOptions Console { get; set; } = new() { WriteTo = true };

    public bool UseDefaults { get; set; } = true;

    public SerilogElasticSearchOptions ElasticSearch { get; set; } = new();

    public class SerilogWriteToOptions
    {
        public bool WriteTo { get; set; }
    }

    public class SerilogElasticSearchOptions : SerilogWriteToOptions
    {
        public string ServerUrl { get; set; }

        public string ApiKey { get; set; }
    }
}

public class ElasticAPMOptions
{
    public bool Enabled { get; set; }

    public bool Recording { get; set; }

    public string Environment { get; set; }

    public string ServiceName { get; set; }

    public string ServiceVersion { get; set; }

    public string ServerUrl { get; set; }

    public string SecretToken { get; set; }

    public double TransactionSampleRate { get; set; }

    public string CaptureBody { get; set; }

    public string CaptureBodyContentTypes { get; set; }

    public bool CaptureHeaders { get; set; }
}

// TODO: Use Serilog based on AppSettings, make this code work
public static class SerilogExtensions
{
    public static IHostBuilder UseSerilog(this IHostBuilder builder, Action<SerilogOptions> serilogOptionsAction = null,
        Action<LoggerConfiguration> customAditionalLoggerConfigurationAction = null,
        string configurationKey = "Serilog")
    {
        builder.ConfigureLogging(delegate(HostBuilderContext hb, ILoggingBuilder lb)
        {
            if (Log.Logger is not ReloadableLogger)
            {
                var loggerConfiguration2 = new LoggerConfiguration().ReadFrom.Configuration(hb.Configuration).Enrich
                    .FromLogContext();
                loggerConfiguration2.WriteTo.Console();
                loggerConfiguration2.MinimumLevel.Is(LogEventLevel.Verbose);
                Log.Logger = loggerConfiguration2.CreateBootstrapLogger();
            }
        });
        builder.ConfigureServices(delegate(IServiceCollection services)
        {
            var optionsBuilder = services.AddOptions<SerilogOptions>().Configure(
                delegate(SerilogOptions options, IConfiguration configuration, IHostEnvironment hostEnv,
                    IServiceProvider serviceProvider)
                {
                    options.ElasticSearch.WriteTo = !hostEnv.IsDevelopment();
                    var service2 = serviceProvider.GetService<IOptions<ElasticAPMOptions>>();
                    options.APMIntegrationEnabled = service2 != null && service2.Value?.Enabled == true &&
                                                    service2 != null && service2.Value?.Recording == true;
                    configuration.GetSection(configurationKey).Bind(options);
                });
            if (serilogOptionsAction != null)
            {
                optionsBuilder.Configure(serilogOptionsAction);
            }
        });
        builder.UseSerilog(delegate(HostBuilderContext context, IServiceProvider services,
            LoggerConfiguration loggerConfiguration)
        {
            var serilogOptions = services.GetRequiredService<IOptions<SerilogOptions>>().Value;
            var requiredService = services.GetRequiredService<IConfiguration>();
            if (serilogOptions.UseDefaults)
            {
                loggerConfiguration.Enrich.FromLogContext().Enrich
                    .WithProperty("Environment", context.HostingEnvironment.EnvironmentName).Enrich.WithMachineName();
                if (serilogOptions.APMIntegrationEnabled)
                {
                    loggerConfiguration.Enrich.WithElasticApmCorrelationInfo();
                }

                var outputTemplate = serilogOptions.APMIntegrationEnabled
                    ? "[{ElasticApmTraceId} {ElasticApmTransactionId} {Message:lj} {NewLine}{Exception}"
                    : "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}";
                if (serilogOptions.Console.WriteTo)
                {
                    loggerConfiguration.WriteTo.Console(LogEventLevel.Verbose, outputTemplate);
                }

                if (serilogOptions.ElasticSearch.WriteTo &&
                    !string.IsNullOrEmpty(serilogOptions.ElasticSearch.ServerUrl))
                {
                    ElasticsearchSinkOptions elasticsearchSinkOptions =
                        new(new Uri(serilogOptions.ElasticSearch.ServerUrl))
                        {
                            TypeName = null,
                            AutoRegisterTemplate = true,
                            IndexFormat = context.HostingEnvironment.EnvironmentName + "-{0:yyyy.MM.dd}",
                            DetectElasticsearchVersion = true
                        };
                    if (!string.IsNullOrEmpty(serilogOptions.ElasticSearch.ApiKey))
                    {
                        elasticsearchSinkOptions.ModifyConnectionSettings = x =>
                            x.ApiKeyAuthentication(
                                new ApiKeyAuthenticationCredentials(serilogOptions.ElasticSearch.ApiKey));
                    }

                    loggerConfiguration.WriteTo.Elasticsearch(elasticsearchSinkOptions);
                }
            }

            loggerConfiguration.ReadFrom.Configuration(requiredService);
            customAditionalLoggerConfigurationAction?.Invoke(loggerConfiguration);
        });
        return builder;
    }
}

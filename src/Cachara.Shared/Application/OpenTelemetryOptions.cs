namespace Cachara.Shared.Application;

public class OpenTelemetryOptions
{
    public const string Name = "OpenTelemetry";
    public OltpOptions Otlp { get; set; }

    public class OltpOptions
    {
        public string Endpoint { get; set; }
        public string ApiKey { get; set; }
    }
}

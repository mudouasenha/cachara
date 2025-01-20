namespace Cachara.Content.API.API.Options;

public class CacharaContentOptions
{
    public string Name { get; set; }
    public string SqlDb { get; set; }
    public string JobsSqlDb { get; set; }

    public CacharaExporterOptions CacharaExporter { get; set; }

    public CacharaUsersOptions CacharaUsers { get; set; } = new();

    public OpenTelemetryOptions OpenTelemetry { get; set; }

    public SecurityOptions Security { get; set; }
}

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

public class CacharaExporterOptions
{
    public const string Name = "CacharaExporter";

    public string Url { get; set; }

    public string Token { get; set; }
}

public class CacharaUsersOptions
{
    public string ServiceBusConn { get; set; }
    public bool ListenerEnabled { get; set; }
    public int MaxConcurrentCalls { get; set; }
}

public class SecurityOptions
{
    public string Key { get; set; }
}

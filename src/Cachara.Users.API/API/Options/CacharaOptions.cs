
namespace Cachara.Users.API.API.Options
{
    public class CacharaOptions
    {
        public string Name { get; set; }
        
        public string SqlDb { get; set; }
        public string JobsSqlDb { get; set; }

        public CacharaExporterOptions CacharaExporter { get; set; } = new();
        public JwtOptions Jwt { get; set; } = new();

        public CacharaContentOptions CacharaContent { get; set; } = new();

        public OpenTelemetryOptions OpenTelemetry { get; set; } = new();
        
        public SecurityOptions Security { get; set; }
    }

    public class JwtOptions
    {
        public string SecretEnvorinmentVariable { get; set; }
        public string Secret { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public int ExpirationMinutes { get; set; }
    }

    public class OpenTelemetryOptions
    {
        public const string Name  = "OpenTelemetry";
        public OltpOptions Otlp { get; set; }
        
        public class OltpOptions
        {
            public string Endpoint { get; set; }
            public string ApiKey { get; set; }
        }
    }

    public class CacharaExporterOptions
    {
        public const string Name  = "CacharaExporter";
        
        public string Url { get; set; }
        
        public string Token { get; set; }
    }
    
    public class CacharaContentOptions
    {
        public string ServiceBusConn { get; set; }
    }
    
    public class SecurityOptions
    {
        public string Key { get; set; }
    }
}

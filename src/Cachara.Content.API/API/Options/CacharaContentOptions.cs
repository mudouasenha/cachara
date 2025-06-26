using Cachara.Shared.Application;
using Cachara.Shared.Application.Options;

namespace Cachara.Content.API.API.Options;

public class CacharaContentOptions : CacharaOptions
{
    public string SqlDb { get; set; }
    public string JobsSqlDb { get; set; }

    public CacharaUsersOptions CacharaUsers { get; set; } = new();

    public OpenTelemetryOptions OpenTelemetry { get; set; }

    public SecurityOptions Security { get; set; }
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

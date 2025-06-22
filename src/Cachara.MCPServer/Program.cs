using Cachara.Content.MCPServer.Clients;
using Cachara.Content.MCPServer.Tools;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.Protocol;

var builder = Host.CreateApplicationBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole(options =>
{
    options.LogToStandardErrorThreshold = LogLevel.Debug;
});

builder.Configuration.AddEnvironmentVariables();

// TODO: make it work with npx @modelcontextprotocol/inspector
var serverInfo = new Implementation { Name = "CacharaMCPServer", Version = "1.0.0" };
builder.Services
    .AddMcpServer(mcp =>
    {
        mcp.ServerInfo = serverInfo;
    })
    .WithStdioServerTransport()
    .WithPromptsFromAssembly()
    .WithToolsFromAssembly(typeof(PostsTools).Assembly);

builder.Services.AddHttpClient<PostsClient>(client =>
{
    var baseAddress = Environment.GetEnvironmentVariable("API_BASE_ADDRESS");
    if (!string.IsNullOrEmpty(baseAddress))
        client.BaseAddress = new Uri(baseAddress);
    else
        client.BaseAddress = new Uri("https://localhost:7294/api/");
});

var app = builder.Build();
await app.RunAsync();

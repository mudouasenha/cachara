using Cachara.Content.API;
using Cachara.Content.API.API.Options;
using Cachara.Shared.Application;

var builder = WebApplication.CreateBuilder(args);
var service = new CacharaContentService(builder.Environment, builder.Configuration);
//var logging = new CacharaLogging<CacharaOptions>(builder.Environment, builder.Configuration);

builder.Host.ConfigureServices(service.ConfigureServices);
builder.Logging.ConfigureOpenTelemetry(builder.Environment, builder.Configuration); // TODO: Use CacharaLogging Class

var app = builder.Build();
service.ConfigureApp(app);

app.Run();

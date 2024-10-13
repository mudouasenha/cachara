using Cachara.Content.API;
using Cachara.Content.API.Extensions;
using Cachara.Content.API.Options;

var builder = WebApplication.CreateBuilder(args);
var service = new CacharaContentService<CacharaContentOptions>(builder.Environment, builder.Configuration);
//var logging = new CacharaLogging<CacharaOptions>(builder.Environment, builder.Configuration);

builder.Host.ConfigureServices(service.ConfigureServices);
builder.Logging.ConfigureOpenTelemetry(builder.Environment, builder.Configuration); // TODO: Use CacharaLogging Class

var app = builder.Build();
service.ConfigureApp(app);

app.Run();

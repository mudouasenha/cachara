using Cachara.API;
using Cachara.API.Extensions;
using Cachara.API.Infrastructure;
using Cachara.API.Options;
using Cachara.CrossCutting;
using Hellang.Middleware.ProblemDetails;

var builder = WebApplication.CreateBuilder(args);
var service = new CacharaService<CacharaOptions>(builder.Environment, builder.Configuration);
//var logging = new CacharaLogging<CacharaOptions>(builder.Environment, builder.Configuration);

builder.Host.ConfigureServices(service.ConfigureServices);
builder.Logging.ConfigureOpenTelemetry(); // TODO: Use CacharaLogging Class

var app = builder.Build();
service.ConfigureApp(app);

app.Run();

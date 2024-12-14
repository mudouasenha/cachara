using Cachara.Users.API;
using Cachara.Users.API.API.Extensions;
using Cachara.Users.API.API.Options;

var builder = WebApplication.CreateBuilder(args);
var service = new CacharaUsersService<CacharaOptions>(builder.Environment, builder.Configuration);
//var logging = new CacharaLogging<CacharaOptions>(builder.Environment, builder.Configuration);

builder.Host.ConfigureServices(service.ConfigureServices);
builder.Logging.ConfigureOpenTelemetry(builder.Environment, builder.Configuration); // TODO: Use CacharaLogging Class

var app = builder.Build();
service.ConfigureApp(app);

app.Run();

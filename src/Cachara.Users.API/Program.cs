using Cachara.Shared.Application;
using Cachara.Users.API;
using Cachara.Users.API.API.Options;

var builder = WebApplication.CreateBuilder(args);
var service = new CacharaUsersService(builder.Environment, builder.Configuration);

builder.Host.ConfigureServices(service.ConfigureServices);
builder.Logging.ConfigureOpenTelemetry(builder.Environment, builder.Configuration)
    .ConfigureSerilog(builder.Environment, builder.Configuration);

var app = builder.Build();
service.ConfigureApp(app);

app.Run();

using Cachara.Content.API;
using Cachara.Shared.Application;
using Cachara.Shared.Infrastructure;
using Cachara.Shared.Infrastructure.Logging;

var builder = WebApplication.CreateBuilder(args);
var service = new CacharaContentService(builder.Environment, builder.Configuration);

builder.Host.ConfigureServices(service.ConfigureServices);
builder.Logging.ConfigureLogging(builder.Environment, builder.Configuration);

var app = builder.Build();
service.Configure(app);

app.Run();

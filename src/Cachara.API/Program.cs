using Cachara.API;
using Cachara.API.Extensions;
using Cachara.API.Infrastructure;
using Cachara.API.Options;
using Cachara.CrossCutting;
using Hellang.Middleware.ProblemDetails;

var builder = WebApplication.CreateBuilder(args);
var service = new CacharaService<CacharaOptions>(builder.Environment, builder.Configuration);

//builder.Services.AddSerilog();
builder.Host.ConfigureServices(service.ConfigureServices);

var app = builder.Build();
service.ConfigureApp(app);

app.Run();

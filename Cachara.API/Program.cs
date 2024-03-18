using Cachara.API.Extensions;
using Cachara.API.Infrastructure;
using Cachara.CrossCutting;
using Hellang.Middleware.ProblemDetails;

var builder = WebApplication.CreateBuilder(args);


//builder.Services.AddSerilog();
builder.Services.AddCrossCutting(builder.Configuration);

builder.Services.AddProblemDetails();

builder.Services.AddControllers(options =>
{
    options.Conventions.Add(new ApiExplorerGroupPerVersionConvention());

    options.InputFormatters.Add(new TextPlainInputFormatter());
    options.InputFormatters.Add(new StreamInputFormatter());
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddCustomSwagger();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseProblemDetails();
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

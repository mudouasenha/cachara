using Cachara.Shared.Application.Errors;
using Cachara.Shared.Infrastructure.Middlewares;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Cachara.Shared.Application;

public abstract class CacharaService<TOptions> where TOptions : CacharaOptions, new()
{
    protected readonly IConfiguration Configuration;
    protected readonly IHostEnvironment Environment;
    protected TOptions Options { get; }

    protected CacharaService(IHostEnvironment environment, IConfiguration configuration)
    {
        Environment = environment;
        Configuration = configuration;
        Options = new TOptions { Name = GetType().Name };

        try
        {
            Configuration.Bind(Options);
        }
        catch (Exception)
        {
            Console.WriteLine($"Could not Bind Options for {GetType().Name}");
            throw;
        }
    }

    public virtual void Configure(IApplicationBuilder app)
    {
        ConfigureApp(app);
    }

    public virtual void ConfigureServices(IServiceCollection services)
    {
        services.AddOptions<TOptions>().Bind(Configuration);


        // Exception Handlers
        services.AddScoped<IAggregateExceptionHandler, AggregateExceptionHandler>();
        services.AddScoped<IErrorExceptionHandler<Exception>, ExceptionHandler>();
    }

    protected virtual void ConfigureApp(IApplicationBuilder app)
    {
        app.UseMiddleware<RequestTracingMiddleware>();

    }
}

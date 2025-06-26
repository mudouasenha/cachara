using Cachara.Shared.Application.Errors;
using Cachara.Shared.Application.Options;
using Cachara.Shared.Infrastructure.Middlewares;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Hellang.Middleware.ProblemDetails;

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

        services.Configure<RouteOptions>(options =>
        {
            options.LowercaseUrls = true;
        });

        services.AddProblemDetails(options =>
        {
            options.IncludeExceptionDetails = (ctx, ex) => Environment.IsDevelopment();
            options.Map<ValidationException>(ex => new StatusCodeProblemDetails(400)
            {
                Title = ex.Message,
                Detail = ex.Message,
                Status = StatusCodes.Status400BadRequest
            });

            options.Map<UnauthorizedAccessException>(ex => new ProblemDetails
            {
                Title = "Unauthorized",
                Status = StatusCodes.Status401Unauthorized,
                Detail = ex.Message,
            });

            options.Map<NotFoundException>(ex => new ProblemDetails
            {
                Title = "Not Found",
                Status = StatusCodes.Status404NotFound,
                Detail = ex.Message,
            });
        });

        // Exception Handlers
        services.AddScoped<IAggregateExceptionHandler, AggregateExceptionHandler>();
        services.AddScoped<IErrorExceptionHandler<Exception>, ExceptionHandler>();
        services.AddScoped<IErrorExceptionHandler<NotFoundException>, NotFoundExceptionHandler>();
    }

    protected virtual void ConfigureApp(IApplicationBuilder app)
    {
        app.UseMiddleware<RequestTracingMiddleware>();
        app.UseProblemDetails();
    }
}

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Cachara.Shared.Infrastructure.Middlewares;

public class ExceptionHandlingLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingLoggingMiddleware> _logger;

    public ExceptionHandlingLoggingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            var correlationId = context.TraceIdentifier;

            _logger.LogError(ex,
                "Unhandled exception occurred. CorrelationId: {CorrelationId}, Path: {Path}",
                correlationId, context.Request.Path);

            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await context.Response.WriteAsJsonAsync(new
            {
                Error = "An unexpected error occurred.",
                CorrelationId = correlationId
            });
        }
    }
}


using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Cachara.Shared.Infrastructure.Middlewares;

public class RequestResponseLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestResponseLoggingMiddleware> _logger;
    private const string CorrelationIdHeader = "X-Correlation-ID";

    public RequestResponseLoggingMiddleware(RequestDelegate next, ILogger<RequestResponseLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        var correlationId = context.Request.Headers[CorrelationIdHeader].FirstOrDefault()
                            ?? context.TraceIdentifier;

        context.Response.Headers[CorrelationIdHeader] = correlationId;

        var method = context.Request.Method;
        var path = context.Request.Path;

        using (_logger.BeginScope(new { CorrelationId = correlationId, Method = method, Path = path }))
        {
            _logger.LogInformation("Request started: {Method} {Path}", method, path);

            var stopwatch = Stopwatch.StartNew();

            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Request failed: {Method} {Path}", method, path);
                throw;
            }
            finally
            {
                stopwatch.Stop();
                _logger.LogInformation("Request completed: {Method} {Path} - StatusCode: {StatusCode} - ElapsedMs: {ElapsedMs}",
                    method, path, context.Response.StatusCode, stopwatch.ElapsedMilliseconds);
            }
        }
    }



}


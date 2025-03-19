using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Trace;

namespace Cachara.Shared.Infrastructure.Middlewares;

public class RequestTracingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestTracingMiddleware> _logger;
    private const string CorrelationIdHeader = "X-Correlation-ID";

    public RequestTracingMiddleware(RequestDelegate next, ILogger<RequestTracingMiddleware> logger)
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

        var userId = context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "Anonymous";
        var tenantId = context.Request.Headers["X-Tenant-ID"].FirstOrDefault() ?? "Unknown";

        // Ensure correlation ID and key data are tagged in the trace context before anything else
        var activity = Activity.Current;
        if (activity is not null)
        {
            activity.SetTag("correlation_id", correlationId);
            activity.SetTag("user.id", userId);
            activity.SetTag("tenant.id", tenantId);
        }

        using (_logger.BeginScope(new
               {
                   CorrelationId = correlationId,
                   UserId = userId,
                   TenantId = tenantId
               }))
        {
            _logger.LogInformation("Request started: {Method} {Path}", method, path);

            var stopwatch = Stopwatch.StartNew();

            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                activity?.AddException(ex);

                _logger.LogError(
                    ex,
                    "Unhandled exception occurred. CorrelationId: {CorrelationId}, Method: {Method} - Path: {Path}",
                    correlationId,
                    method,
                    path);
                throw;
            }
            finally
            {
                stopwatch.Stop();
                if (context.Response.StatusCode >= 400)
                {
                    _logger.LogWarning("Client error: {Method} {Path} - StatusCode: {StatusCode}",
                        method, path, context.Response.StatusCode);
                }
                _logger.LogInformation(
                    "Request finished: {Method} {Path} - StatusCode: {StatusCode} - ElapsedMs: {ElapsedMs}",
                    method, path, context.Response.StatusCode, stopwatch.ElapsedMilliseconds);
            }
        }
    }




}


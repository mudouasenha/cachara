using Cachara.Users.API.API.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Cachara.Shared.Infrastructure.Middlewares;

public class SessionValidationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<SessionValidationMiddleware> _logger;
    private const string _sessionValidationPathException = "/public/auth/register";

    public SessionValidationMiddleware(RequestDelegate next, IServiceProvider serviceProvider, ILogger<SessionValidationMiddleware> logger)
    {
        _next = next;
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Path.StartsWithSegments(_sessionValidationPathException))
        {
            _logger.LogDebug("Session validation bypassed for path {Path}", context.Request.Path);
            await _next(context); // Important: we must continue the pipeline
            return;
        }

        var sessionId = context.Request.Headers["X-Session-ID"];

        if (string.IsNullOrEmpty(sessionId))
        {
            _logger.LogWarning("Missing session ID for request {Path}", context.Request.Path);
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync("Session ID is missing.");
            return;
        }

        using var scope = _serviceProvider.CreateScope();
        var sessionStore = scope.ServiceProvider.GetRequiredService<ISessionStoreService<UserAccount>>();
        var session = await sessionStore.GetSession(sessionId);

        // Log invalid session
        if (session == null)
        {
            _logger.LogWarning("Invalid or expired session for session ID {SessionId}, request {Path}", sessionId, context.Request.Path);
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync("Session expired or invalid.");
            return;
        }

        // Log successful validation at debug level
        _logger.LogDebug("Session {SessionId} validated successfully for request {Path}", sessionId, context.Request.Path);

        await _next(context);
    }
}

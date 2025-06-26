using Cachara.Shared.Application.Abstractions;
using Cachara.Shared.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Cachara.Shared.Infrastructure.Middlewares;

public class SessionValidationMiddleware(
    RequestDelegate next,
    IServiceProvider serviceProvider,
    ILogger<SessionValidationMiddleware> logger)
{
    private static readonly string[] _sessionValidationPathExceptions = ["/public/auth/register", "/public/auth/login", "/"];


    public async Task InvokeAsync(HttpContext context)
    {
        logger.LogDebug("Session validation IS BEING BYPASSED FOR ALL PATHS, PLEASE FIX! path {Path}", context.Request.Path);
        await next(context);
        return;

        if (_sessionValidationPathExceptions.Any(path => context.Request.Path.StartsWithSegments(path)))
        {
            logger.LogDebug("Session validation bypassed for path {Path}", context.Request.Path);
            await next(context);
            return;
        }

        var sessionId = context.Request.Headers["X-Session-ID"];

        if (string.IsNullOrEmpty(sessionId))
        {
            logger.LogWarning("Missing session ID for request {Path}", context.Request.Path);
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync("Session ID is missing.");
            return;
        }

        using var scope = serviceProvider.CreateScope();
        var sessionStore = scope.ServiceProvider.GetRequiredService<ISessionStoreService<UserAccount>>();
        var session = await sessionStore.GetSession(sessionId);

        if (session == null)
        {
            logger.LogWarning("Invalid or expired session for session ID {SessionId}, request {Path}", sessionId, context.Request.Path);
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync("Session expired or invalid.");
            return;
        }

        logger.LogDebug("Session {SessionId} validated successfully for request {Path}", sessionId, context.Request.Path);

        await next(context);
    }
}

using Microsoft.AspNetCore.Http;

namespace Cachara.Shared.Infrastructure.Middlewares;

public class SessionValidationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ISessionStoreService<IAccount> _sessionStore;

    public SessionValidationMiddleware(RequestDelegate next, ISessionStoreService<IAccount> sessionStore)
    {
        _next = next;
        _sessionStore = sessionStore;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var sessionId = context.Request.Headers["X-Session-ID"];
        if (string.IsNullOrEmpty(sessionId))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync("Session ID is missing.");
            return;
        }

        var session = await _sessionStore.GetSession(sessionId);
        if (session == null)
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync("Session expired or invalid.");
            return;
        }

        await _next(context);
    }
}


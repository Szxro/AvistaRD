using System.Security.Claims;
using AvistaRD.Application.Contracts;

namespace AvistaRD.Api.Middlewares;

// Identity bearer tokens are self-contained: on their own they stay valid until they expire (1 hour).
// This middleware checks every authenticated request against the database. If the account was deactivated,
// or its security stamp changed after the token was issued (deactivation, role change), the request goes on
// as anonymous, so [Authorize] endpoints answer 401.
public class SessionValidationMiddleware : IMiddleware
{
    private readonly IAuthService _authService;

    public SessionValidationMiddleware(IAuthService authService)
    {
        _authService = authService;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        if (context.User.Identity?.IsAuthenticated == true
            && !await _authService.IsSessionValidAsync(context.User, context.RequestAborted))
        {
            context.User = new ClaimsPrincipal(new ClaimsIdentity());
        }

        await next(context);
    }
}

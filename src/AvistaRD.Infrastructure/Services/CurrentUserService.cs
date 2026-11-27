using System.Security.Claims;
using AvistaRD.Application.Contracts;
using AvistaRD.Infrastructure.Common.Attributes;
using AvistaRD.Infrastructure.Common.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace AvistaRD.Infrastructure.Services;

[Inject(ServiceKind.Service, ServiceLifetime.Scoped)]
public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContext;

    public CurrentUserService(IHttpContextAccessor httpContext)
    {
        _httpContext = httpContext;
    }
    
    // Claims issued by Identity (UserClaimsPrincipalFactory): the name is the email and the id a GUID string
    public string? UserName
        => _httpContext?.HttpContext?.User.FindFirstValue(ClaimTypes.Name);

    public string? UserId
        => _httpContext?.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

    public string IpAddress =>
        _httpContext?.HttpContext?.Connection.RemoteIpAddress?.ToString() ?? "unknown";

    public string UserAgent => 
        _httpContext?.HttpContext?.Request.Headers.UserAgent.ToString() ??  "unknown";
}
using System.Security.Claims;
using AvistaRD.Application.Contracts;
using AvistaRD.Application.Features.Auth;
using AvistaRD.Domain.Users;
using AvistaRD.Infrastructure.Common.Attributes;
using AvistaRD.Infrastructure.Common.Enums;
using AvistaRD.Infrastructure.Identity;
using AvistaRD.SharedKernel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace AvistaRD.Infrastructure.Services;

[Inject(ServiceKind.Service, ServiceLifetime.Scoped)]
public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IOptionsMonitor<BearerTokenOptions> _bearerTokenOptions;
    private readonly TimeProvider _timeProvider;

    public AuthService(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IOptionsMonitor<BearerTokenOptions> bearerTokenOptions,
        TimeProvider timeProvider)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _bearerTokenOptions = bearerTokenOptions;
        _timeProvider = timeProvider;
    }

    public async Task<Result<ClaimsPrincipal>> AuthenticateAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        ApplicationUser? user = await _userManager.FindByEmailAsync(email);

        if (user is null)
        {
            return Result<ClaimsPrincipal>.Failure(AuthErrors.InvalidCredentials);
        }

        // Checks the lockout and the password, counting failed attempts (by default 5 failures lock the account 5 minutes)
        SignInResult signInResult = await _signInManager.CheckPasswordSignInAsync(user, password, lockoutOnFailure: true);

        if (signInResult.IsLockedOut)
        {
            return Result<ClaimsPrincipal>.Failure(AuthErrors.LockedOut);
        }

        if (!signInResult.Succeeded)
        {
            return Result<ClaimsPrincipal>.Failure(AuthErrors.InvalidCredentials);
        }

        // Checked after the password, so only the account owner learns that it's pending or deactivated
        if (user.Status == UserStatus.Pending)
        {
            return Result<ClaimsPrincipal>.Failure(AuthErrors.AccountPending);
        }

        if (user.Status == UserStatus.Deactivated)
        {
            return Result<ClaimsPrincipal>.Failure(AuthErrors.AccountDeactivated);
        }

        // Carries the user id, email, roles and security stamp
        ClaimsPrincipal principal = await _signInManager.CreateUserPrincipalAsync(user);

        return Result<ClaimsPrincipal>.Success(principal);
    }

    public async Task<Result<ClaimsPrincipal>> RefreshAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        ISecureDataFormat<AuthenticationTicket> refreshTokenProtector =
            _bearerTokenOptions.Get(IdentityConstants.BearerScheme).RefreshTokenProtector;

        AuthenticationTicket? refreshTicket = refreshTokenProtector.Unprotect(refreshToken);

        // Same checks as MapIdentityApi's /refresh (expiration and security stamp), plus the account status
        if (refreshTicket?.Properties.ExpiresUtc is not { } expiresUtc
            || _timeProvider.GetUtcNow() >= expiresUtc
            || await _signInManager.ValidateSecurityStampAsync(refreshTicket.Principal) is not { Status: UserStatus.Active } user)
        {
            return Result<ClaimsPrincipal>.Failure(AuthErrors.InvalidRefreshToken);
        }

        ClaimsPrincipal principal = await _signInManager.CreateUserPrincipalAsync(user);

        return Result<ClaimsPrincipal>.Success(principal);
    }

    public async Task<bool> IsSessionValidAsync(ClaimsPrincipal principal, CancellationToken cancellationToken = default)
    {
        // Null when the account doesn't exist or its security stamp no longer matches the token's
        ApplicationUser? user = await _signInManager.ValidateSecurityStampAsync(principal);

        return user is { Status: UserStatus.Active };
    }
}

using System.Security.Claims;
using AvistaRD.SharedKernel;

namespace AvistaRD.Application.Contracts;

public interface IAuthService
{
    // Checks the password (counting failed attempts for lockout) and the account status.
    // Returns the principal the API issues the Identity bearer tokens for.
    Task<Result<ClaimsPrincipal>> AuthenticateAsync(string email, string password, CancellationToken cancellationToken = default);

    // Validates a refresh token and returns a fresh principal for its account
    Task<Result<ClaimsPrincipal>> RefreshAsync(string refreshToken, CancellationToken cancellationToken = default);

    // False when the account behind an authenticated request is no longer active, or its security stamp
    // changed (deactivation, role change) after the token was issued
    Task<bool> IsSessionValidAsync(ClaimsPrincipal principal, CancellationToken cancellationToken = default);
}

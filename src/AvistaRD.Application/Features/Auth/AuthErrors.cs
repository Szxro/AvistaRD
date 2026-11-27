using AvistaRD.SharedKernel;

namespace AvistaRD.Application.Features.Auth;

public static class AuthErrors
{
    public static readonly Error InvalidCredentials = Error.Unauthorized(
        "Auth.InvalidCredentials",
        "The email or password is incorrect.");

    public static readonly Error LockedOut = Error.Forbidden(
        "Auth.LockedOut",
        "The account is temporarily locked after too many failed sign-in attempts. Try again later.");

    public static readonly Error AccountPending = Error.Forbidden(
        "Auth.AccountPending",
        "The account is waiting for an administrator's approval.");

    public static readonly Error AccountDeactivated = Error.Forbidden(
        "Auth.AccountDeactivated",
        "The account was deactivated by an administrator.");

    public static readonly Error InvalidRefreshToken = Error.Unauthorized(
        "Auth.InvalidRefreshToken",
        "The refresh token is invalid or expired. Sign in again.");

    public static readonly Error NotAuthenticated = Error.Unauthorized(
        "Auth.NotAuthenticated",
        "The request isn't authenticated.");
}

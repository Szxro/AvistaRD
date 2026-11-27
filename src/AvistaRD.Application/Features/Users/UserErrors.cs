using AvistaRD.SharedKernel;

namespace AvistaRD.Application.Features.Users;

public static class UserErrors
{
    public static readonly Error NotFound = Error.NotFound(
        "Users.NotFound",
        "The user account doesn't exist.");

    public static readonly Error EmailAlreadyRegistered = Error.Conflict(
        "Users.EmailAlreadyRegistered",
        "An account with this email already exists.");

    public static readonly Error NotPending = Error.Conflict(
        "Users.NotPending",
        "Only pending accounts can be approved.");

    public static readonly Error AlreadyDeactivated = Error.Conflict(
        "Users.AlreadyDeactivated",
        "The account is already deactivated.");

    public static readonly Error NotDeactivated = Error.Conflict(
        "Users.NotDeactivated",
        "Only deactivated accounts can be reactivated.");

    public static readonly Error CannotDeactivateSelf = Error.Forbidden(
        "Users.CannotDeactivateSelf",
        "You can't deactivate your own account.");

    public static readonly Error CannotRemoveOwnAdministratorRole = Error.Forbidden(
        "Users.CannotRemoveOwnAdministratorRole",
        "You can't remove the Administrador role from your own account.");
}

namespace AvistaRD.Domain.Users;

public enum UserStatus
{
    // Waiting for an administrator's approval; can't sign in yet
    Pending = 0,

    // Can sign in
    Active = 1,

    // Disabled by an administrator: can't sign in and its tokens are rejected, but its records are kept
    Deactivated = 2
}

using AvistaRD.Domain.Users;

namespace AvistaRD.Application.Features.Users;

public sealed record UserResponse(
    string Id,
    string Email,
    string FullName,
    string? Role,
    UserStatus Status,
    DateTime CreatedAt);

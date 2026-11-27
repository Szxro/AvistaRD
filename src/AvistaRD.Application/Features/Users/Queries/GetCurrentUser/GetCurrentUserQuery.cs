using AvistaRD.Application.Common.Abstractions;

namespace AvistaRD.Application.Features.Users.Queries.GetCurrentUser;

// Profile of the signed-in user. Identity bearer tokens can't be decoded by the frontend,
// so this is how it learns the user's role.
public sealed record GetCurrentUserQuery : IQuery<UserResponse>;

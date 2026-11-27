using AvistaRD.Application.Common.Abstractions;

namespace AvistaRD.Application.Features.Users.Queries.GetUserById;

public sealed record GetUserByIdQuery(string UserId) : IQuery<UserResponse>;

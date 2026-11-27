using AvistaRD.Application.Common.Abstractions;

namespace AvistaRD.Application.Features.Users.Commands.ReactivateUser;

public sealed record ReactivateUserCommand(string UserId) : ICommand<UserResponse>;

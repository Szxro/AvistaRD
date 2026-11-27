using AvistaRD.Application.Common.Abstractions;

namespace AvistaRD.Application.Features.Users.Commands.UpdateUser;

public sealed record UpdateUserCommand(string UserId, string FullName, string Role) : ICommand<UserResponse>;

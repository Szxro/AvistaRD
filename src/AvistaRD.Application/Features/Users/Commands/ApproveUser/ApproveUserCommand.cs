using AvistaRD.Application.Common.Abstractions;

namespace AvistaRD.Application.Features.Users.Commands.ApproveUser;

public sealed record ApproveUserCommand(string UserId) : ICommand<UserResponse>;

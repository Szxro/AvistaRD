using AvistaRD.Application.Common.Abstractions;

namespace AvistaRD.Application.Features.Users.Commands.DeactivateUser;

// Works on active accounts and on pending ones (rejecting the request). Nothing is deleted.
public sealed record DeactivateUserCommand(string UserId) : ICommand<UserResponse>;

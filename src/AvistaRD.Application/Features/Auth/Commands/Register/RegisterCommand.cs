using AvistaRD.Application.Common.Abstractions;
using AvistaRD.Application.Features.Users;

namespace AvistaRD.Application.Features.Auth.Commands.Register;

// Role is optional and defaults to Ciudadano
public sealed record RegisterCommand(string Email, string Password, string FullName, string? Role) : ICommand<UserResponse>;

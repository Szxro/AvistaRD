using System.Security.Claims;
using AvistaRD.Application.Common.Abstractions;

namespace AvistaRD.Application.Features.Auth.Commands.Login;

public sealed record LoginCommand(string Email, string Password) : ICommand<ClaimsPrincipal>;

using System.Security.Claims;
using AvistaRD.Application.Common.Abstractions;

namespace AvistaRD.Application.Features.Auth.Commands.RefreshToken;

public sealed record RefreshTokenCommand(string RefreshToken) : ICommand<ClaimsPrincipal>;

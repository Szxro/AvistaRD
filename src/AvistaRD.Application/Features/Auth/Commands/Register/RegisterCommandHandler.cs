using AvistaRD.Application.Common.Abstractions;
using AvistaRD.Application.Contracts;
using AvistaRD.Application.Features.Users;
using AvistaRD.Domain.Users;
using AvistaRD.SharedKernel;

namespace AvistaRD.Application.Features.Auth.Commands.Register;

public sealed class RegisterCommandHandler : ICommandHandler<RegisterCommand, UserResponse>
{
    private readonly IUserAccountService _userAccountService;

    public RegisterCommandHandler(IUserAccountService userAccountService)
    {
        _userAccountService = userAccountService;
    }

    public async Task<Result<UserResponse>> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        string role = string.IsNullOrEmpty(request.Role) ? Roles.Citizen : request.Role;

        // Citizens can sign in right away; Validador and Guardaparques wait for an administrator
        UserStatus status = Roles.RequiresApproval(role) ? UserStatus.Pending : UserStatus.Active;

        return await _userAccountService.CreateAsync(
            request.Email.Trim(),
            request.Password,
            request.FullName.Trim(),
            role,
            status,
            cancellationToken);
    }
}

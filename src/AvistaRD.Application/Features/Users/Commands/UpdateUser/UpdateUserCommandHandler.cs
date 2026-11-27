using AvistaRD.Application.Common.Abstractions;
using AvistaRD.Application.Contracts;
using AvistaRD.Domain.Users;
using AvistaRD.SharedKernel;

namespace AvistaRD.Application.Features.Users.Commands.UpdateUser;

public sealed class UpdateUserCommandHandler : ICommandHandler<UpdateUserCommand, UserResponse>
{
    private readonly IUserAccountService _userAccountService;
    private readonly ICurrentUserService _currentUserService;

    public UpdateUserCommandHandler(
        IUserAccountService userAccountService,
        ICurrentUserService currentUserService)
    {
        _userAccountService = userAccountService;
        _currentUserService = currentUserService;
    }

    public async Task<Result<UserResponse>> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        UserResponse? user = await _userAccountService.GetByIdAsync(request.UserId, cancellationToken);

        if (user is null)
        {
            return Result<UserResponse>.Failure(UserErrors.NotFound);
        }

        // An administrator demoting themselves could leave the platform without administrators
        if (user.Id == _currentUserService.UserId && request.Role != Roles.Administrator)
        {
            return Result<UserResponse>.Failure(UserErrors.CannotRemoveOwnAdministratorRole);
        }

        // If the role changes, the user's tokens stop working and they have to sign in again
        UserResponse updated = await _userAccountService.UpdateAsync(
            request.UserId,
            request.FullName.Trim(),
            request.Role,
            cancellationToken);

        return Result<UserResponse>.Success(updated);
    }
}

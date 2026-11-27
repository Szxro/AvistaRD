using AvistaRD.Application.Common.Abstractions;
using AvistaRD.Application.Contracts;
using AvistaRD.Domain.Users;
using AvistaRD.SharedKernel;

namespace AvistaRD.Application.Features.Users.Commands.DeactivateUser;

public sealed class DeactivateUserCommandHandler : ICommandHandler<DeactivateUserCommand, UserResponse>
{
    private readonly IUserAccountService _userAccountService;
    private readonly ICurrentUserService _currentUserService;

    public DeactivateUserCommandHandler(
        IUserAccountService userAccountService,
        ICurrentUserService currentUserService)
    {
        _userAccountService = userAccountService;
        _currentUserService = currentUserService;
    }

    public async Task<Result<UserResponse>> Handle(DeactivateUserCommand request, CancellationToken cancellationToken)
    {
        UserResponse? user = await _userAccountService.GetByIdAsync(request.UserId, cancellationToken);

        if (user is null)
        {
            return Result<UserResponse>.Failure(UserErrors.NotFound);
        }

        // Also guarantees there is always at least one active administrator
        if (user.Id == _currentUserService.UserId)
        {
            return Result<UserResponse>.Failure(UserErrors.CannotDeactivateSelf);
        }

        if (user.Status == UserStatus.Deactivated)
        {
            return Result<UserResponse>.Failure(UserErrors.AlreadyDeactivated);
        }

        UserResponse deactivated = await _userAccountService.ChangeStatusAsync(request.UserId, UserStatus.Deactivated, cancellationToken);

        return Result<UserResponse>.Success(deactivated);
    }
}

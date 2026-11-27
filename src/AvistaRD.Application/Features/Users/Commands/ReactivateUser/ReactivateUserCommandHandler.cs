using AvistaRD.Application.Common.Abstractions;
using AvistaRD.Application.Contracts;
using AvistaRD.Domain.Users;
using AvistaRD.SharedKernel;

namespace AvistaRD.Application.Features.Users.Commands.ReactivateUser;

public sealed class ReactivateUserCommandHandler : ICommandHandler<ReactivateUserCommand, UserResponse>
{
    private readonly IUserAccountService _userAccountService;

    public ReactivateUserCommandHandler(IUserAccountService userAccountService)
    {
        _userAccountService = userAccountService;
    }

    public async Task<Result<UserResponse>> Handle(ReactivateUserCommand request, CancellationToken cancellationToken)
    {
        UserResponse? user = await _userAccountService.GetByIdAsync(request.UserId, cancellationToken);

        if (user is null)
        {
            return Result<UserResponse>.Failure(UserErrors.NotFound);
        }

        if (user.Status != UserStatus.Deactivated)
        {
            return Result<UserResponse>.Failure(UserErrors.NotDeactivated);
        }

        UserResponse reactivated = await _userAccountService.ChangeStatusAsync(request.UserId, UserStatus.Active, cancellationToken);

        return Result<UserResponse>.Success(reactivated);
    }
}

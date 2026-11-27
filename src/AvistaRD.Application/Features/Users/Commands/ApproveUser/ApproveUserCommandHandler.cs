using AvistaRD.Application.Common.Abstractions;
using AvistaRD.Application.Contracts;
using AvistaRD.Domain.Users;
using AvistaRD.SharedKernel;

namespace AvistaRD.Application.Features.Users.Commands.ApproveUser;

public sealed class ApproveUserCommandHandler : ICommandHandler<ApproveUserCommand, UserResponse>
{
    private readonly IUserAccountService _userAccountService;

    public ApproveUserCommandHandler(IUserAccountService userAccountService)
    {
        _userAccountService = userAccountService;
    }

    public async Task<Result<UserResponse>> Handle(ApproveUserCommand request, CancellationToken cancellationToken)
    {
        UserResponse? user = await _userAccountService.GetByIdAsync(request.UserId, cancellationToken);

        if (user is null)
        {
            return Result<UserResponse>.Failure(UserErrors.NotFound);
        }

        if (user.Status != UserStatus.Pending)
        {
            return Result<UserResponse>.Failure(UserErrors.NotPending);
        }

        UserResponse approved = await _userAccountService.ChangeStatusAsync(request.UserId, UserStatus.Active, cancellationToken);

        return Result<UserResponse>.Success(approved);
    }
}

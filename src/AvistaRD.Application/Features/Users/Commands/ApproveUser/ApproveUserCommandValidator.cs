using FluentValidation;

namespace AvistaRD.Application.Features.Users.Commands.ApproveUser;

public sealed class ApproveUserCommandValidator : AbstractValidator<ApproveUserCommand>
{
    public ApproveUserCommandValidator()
    {
        RuleFor(command => command.UserId)
            .NotEmpty()
            .MaximumLength(450);
    }
}

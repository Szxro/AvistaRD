using FluentValidation;

namespace AvistaRD.Application.Features.Auth.Commands.Login;

public sealed class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(command => command.Email)
            .NotEmpty();

        RuleFor(command => command.Password)
            .NotEmpty();
    }
}

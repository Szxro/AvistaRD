using AvistaRD.Domain.Users;
using FluentValidation;

namespace AvistaRD.Application.Features.Auth.Commands.Register;

public sealed class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        RuleFor(command => command.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(256);

        // The password policy (length, digits, symbols...) is enforced by Identity
        RuleFor(command => command.Password)
            .NotEmpty();

        RuleFor(command => command.FullName)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(command => command.Role)
            .Must(role => string.IsNullOrEmpty(role) || Roles.Registrable.Contains(role))
            .WithMessage($"Role must be one of: {string.Join(", ", Roles.Registrable)}.");
    }
}

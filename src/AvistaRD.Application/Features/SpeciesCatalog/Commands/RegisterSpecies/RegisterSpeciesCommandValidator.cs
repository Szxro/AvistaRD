using FluentValidation;

namespace AvistaRD.Application.Features.SpeciesCatalog.Commands.RegisterSpecies;

public sealed class RegisterSpeciesCommandValidator : AbstractValidator<RegisterSpeciesCommand>
{
    private const int MaxNameLength = 150;

    public RegisterSpeciesCommandValidator()
    {
        RuleFor(command => command.CommonName)
            .NotEmpty().WithMessage("{PropertyName} can't be empty")
            .MaximumLength(MaxNameLength).WithMessage("{PropertyName} can't exceed {MaxLength} characters");

        RuleFor(command => command.ScientificName)
            .NotEmpty().WithMessage("{PropertyName} can't be empty")
            .MaximumLength(MaxNameLength).WithMessage("{PropertyName} can't exceed {MaxLength} characters");
    }
}

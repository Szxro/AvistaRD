using FluentValidation;

namespace AvistaRD.Application.Features.SpeciesCatalog.Commands.RegisterSpecies;

public sealed class RegisterSpeciesCommandValidator : AbstractValidator<RegisterSpeciesCommand>
{
    public RegisterSpeciesCommandValidator()
    {
        RuleFor(command => command.CommonName).ValidSpeciesName();

        RuleFor(command => command.ScientificName).ValidSpeciesName();
    }
}

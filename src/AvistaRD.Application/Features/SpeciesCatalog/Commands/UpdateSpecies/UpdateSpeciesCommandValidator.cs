using FluentValidation;

namespace AvistaRD.Application.Features.SpeciesCatalog.Commands.UpdateSpecies;

public sealed class UpdateSpeciesCommandValidator : AbstractValidator<UpdateSpeciesCommand>
{
    public UpdateSpeciesCommandValidator()
    {
        RuleFor(command => command.Id)
            .GreaterThan(0).WithMessage("{PropertyName} must be greater than 0");

        RuleFor(command => command.CommonName).ValidSpeciesName();

        RuleFor(command => command.ScientificName).ValidSpeciesName();
    }
}

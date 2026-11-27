using AvistaRD.Application.Common.Abstractions;
using AvistaRD.Application.Contracts.Repositories;
using AvistaRD.Domain;
using AvistaRD.SharedKernel;

namespace AvistaRD.Application.Features.SpeciesCatalog.Commands.UpdateSpecies;

public sealed class UpdateSpeciesCommandHandler : ICommandHandler<UpdateSpeciesCommand>
{
    private readonly ISpeciesRepository _speciesRepository;

    public UpdateSpeciesCommandHandler(ISpeciesRepository speciesRepository)
    {
        _speciesRepository = speciesRepository;
    }

    public async Task<Result> Handle(UpdateSpeciesCommand command, CancellationToken cancellationToken)
    {
        Species? species = await _speciesRepository.GetById(command.Id, cancellationToken);

        if (species is null)
        {
            return Result.Failure(Error.NotFound($"The species with id '{command.Id}' was not found."));
        }

        string scientificName = command.ScientificName.Trim();

        bool nameTakenByAnotherSpecies = await _speciesRepository.AnyAsync(
            other => other.Id != command.Id && other.ScientificName == scientificName,
            cancellationToken);

        if (nameTakenByAnotherSpecies)
        {
            return Result.Failure(
                Error.Conflict($"A species with the scientific name '{scientificName}' already exists."));
        }

        species.CommonName = command.CommonName.Trim();
        species.ScientificName = scientificName;
        species.IsEndemic = command.IsEndemic;
        species.IsSensible = command.IsSensible;

        // Changes are persisted by the transaction pipeline after the handler completes.
        _speciesRepository.Update(species);

        return Result.Success();
    }
}

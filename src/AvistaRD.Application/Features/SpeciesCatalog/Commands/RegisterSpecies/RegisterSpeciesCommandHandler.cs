using AvistaRD.Application.Common.Abstractions;
using AvistaRD.Application.Contracts.Repositories;
using AvistaRD.Domain;
using AvistaRD.SharedKernel;

namespace AvistaRD.Application.Features.SpeciesCatalog.Commands.RegisterSpecies;

public sealed class RegisterSpeciesCommandHandler : ICommandHandler<RegisterSpeciesCommand, int>
{
    private readonly ISpeciesRepository _speciesRepository;
    private readonly IAppDbContext _dbContext;

    public RegisterSpeciesCommandHandler(
        ISpeciesRepository speciesRepository,
        IAppDbContext dbContext)
    {
        _speciesRepository = speciesRepository;
        _dbContext = dbContext;
    }

    public async Task<Result<int>> Handle(RegisterSpeciesCommand command, CancellationToken cancellationToken)
    {
        string scientificName = command.ScientificName.Trim();

        bool alreadyExists = await _speciesRepository.AnyAsync(
            species => species.ScientificName == scientificName,
            cancellationToken);

        if (alreadyExists)
        {
            return Result<int>.Failure(
                Error.Conflict($"A species with the scientific name '{scientificName}' already exists."));
        }

        Species species = new Species
        {
            CommonName = command.CommonName.Trim(),
            ScientificName = scientificName,
            IsEndemic = command.IsEndemic,
            IsSensible = command.IsSensible
        };

        _speciesRepository.Add(species);

        // Saved here (inside the pipeline transaction) so the generated Id can be returned.
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result<int>.Success(species.Id);
    }
}

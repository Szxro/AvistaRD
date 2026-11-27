using AvistaRD.Application.Common.Abstractions;

namespace AvistaRD.Application.Features.SpeciesCatalog.Commands.UpdateSpecies;

public sealed record UpdateSpeciesCommand(
    int Id,
    string CommonName,
    string ScientificName,
    bool IsEndemic,
    bool IsSensible) : ICommand;

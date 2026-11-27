using AvistaRD.Application.Common.Abstractions;

namespace AvistaRD.Application.Features.SpeciesCatalog.Commands.RegisterSpecies;

public sealed record RegisterSpeciesCommand(
    string CommonName,
    string ScientificName,
    bool IsEndemic,
    bool IsSensible) : ICommand<int>;

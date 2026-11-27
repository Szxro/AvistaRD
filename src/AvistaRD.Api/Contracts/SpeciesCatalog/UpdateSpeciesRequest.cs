using AvistaRD.Application.Features.SpeciesCatalog.Commands.UpdateSpecies;

namespace AvistaRD.Api.Contracts.SpeciesCatalog;

public sealed record UpdateSpeciesRequest(
    string CommonName,
    string ScientificName,
    bool IsEndemic,
    bool IsSensible)
{
    public UpdateSpeciesCommand ToCommand(int id) =>
        new(id, CommonName, ScientificName, IsEndemic, IsSensible);
}

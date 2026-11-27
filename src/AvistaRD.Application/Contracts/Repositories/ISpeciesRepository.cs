using AvistaRD.Application.Common.Abstractions;
using AvistaRD.Domain;

namespace AvistaRD.Application.Contracts.Repositories;

public interface ISpeciesRepository : IRepositoryReader<Species>, IRepositoryWriter<Species> { }

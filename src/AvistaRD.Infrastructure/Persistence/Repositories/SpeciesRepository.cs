using AvistaRD.Application.Contracts.Repositories;
using AvistaRD.Domain;
using AvistaRD.Infrastructure.Common;
using AvistaRD.Infrastructure.Common.Attributes;
using AvistaRD.Infrastructure.Common.Enums;
using Microsoft.Extensions.DependencyInjection;

namespace AvistaRD.Infrastructure.Persistence.Repositories;

[Inject(ServiceKind.Repository, ServiceLifetime.Scoped)]
public class SpeciesRepository : GenericRepository<Species>, ISpeciesRepository
{
    public SpeciesRepository(AppDbContext dbContext) : base(dbContext) { }
}

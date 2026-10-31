using System.Data;

namespace AvistaRD.Application.Common.Abstractions;

public interface IAppDbContext
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    
    Task<IDbTransaction> GetDbTransaction(CancellationToken cancellationToken = default);
}
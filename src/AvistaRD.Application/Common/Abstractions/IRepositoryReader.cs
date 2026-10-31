using System.Linq.Expressions;
using AvistaRD.Domain.Common;

namespace AvistaRD.Application.Common.Abstractions;

public interface IRepositoryReader<TEntity>
    where TEntity : Entity
{
    Task<TEntity?> GetById(int id, CancellationToken cancellationToken = default);

    Task<List<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<bool> AnyAsync(Expression<Func<TEntity, bool>> filter, CancellationToken cancellationToken = default);
}
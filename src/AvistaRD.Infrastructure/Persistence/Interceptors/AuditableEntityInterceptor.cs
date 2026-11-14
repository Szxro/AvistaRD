using AvistaRD.Domain.Common;
using AvistaRD.Infrastructure.Common.Attributes;
using AvistaRD.Infrastructure.Common.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace AvistaRD.Infrastructure.Persistence.Interceptors;

[Inject(ServiceKind.Interceptor)]
public class AuditableEntityInterceptor : SaveChangesInterceptor
{
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = new CancellationToken())
    {
        if (eventData.Context is not null)
        {
            UpdateAuditableEntities(eventData.Context);
        }

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }
    
    private void UpdateAuditableEntities(DbContext dbContext)
    {
        List<EntityEntry<AuditableEntity>> entities = dbContext.ChangeTracker.Entries<AuditableEntity>()
            .Where(entity => entity.State is EntityState.Added or EntityState.Modified)
            .ToList();

        foreach (EntityEntry<AuditableEntity> entity in entities)
        {
            if (entity.State == EntityState.Added)
            {
                SetCurrentDatetimeProperty(entity, nameof(AuditableEntity.CreatedAt), DateTime.UtcNow);
                SetCurrentDatetimeProperty(entity, nameof(AuditableEntity.ModifiedAt), DateTime.UtcNow);
            }

            if (entity.State == EntityState.Modified)
            {
                SetCurrentDatetimeProperty(entity, nameof(AuditableEntity.ModifiedAt), DateTime.UtcNow);
            }
        }
        
        void SetCurrentDatetimeProperty(
            EntityEntry entity,
            string propertyName,
            DateTime dateTime) => entity.Property(propertyName).CurrentValue = dateTime;
    }
}
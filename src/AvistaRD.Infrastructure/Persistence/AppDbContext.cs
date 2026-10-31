using System.Data;
using AvistaRD.Application.Common.Abstractions;
using AvistaRD.Domain;
using AvistaRD.Domain.Common;
using AvistaRD.Infrastructure.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace AvistaRD.Infrastructure.Persistence;

public class AppDbContext : IdentityDbContext<IdentityUser>, IAppDbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    
    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Properties<DateTime>().HaveColumnType("datetime2");
        
        base.ConfigureConventions(configurationBuilder);
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        
        modelBuilder.ApplyGlobalQueryFilter<AuditableEntity>(x => !x.IsDeleted);
        
        base.OnModelCreating(modelBuilder);
    }
    
    public DbSet<Alert> Alert => Set<Alert>();
    
    public DbSet<AuditLog> AuditLog => Set<AuditLog>();

    public DbSet<Evidence> Evidence => Set<Evidence>();
    
    public DbSet<Sighting> Sighting => Set<Sighting>();
    
    public DbSet<SightingThreat> SightingThreat => Set<SightingThreat>();
    
    public DbSet<Species> Species => Set<Species>();

    public DbSet<Threat> Threat => Set<Threat>();
    
    public DbSet<Validation> Validation => Set<Validation>();
    
    public DbSet<ValidationStatus> ValidationStatus => Set<ValidationStatus>();
    
    public DbSet<Zone> Zone => Set<Zone>();
    
    public async Task<IDbTransaction> GetDbTransaction(CancellationToken cancellationToken = default)
    {
        IDbContextTransaction transaction = await Database.BeginTransactionAsync(cancellationToken);
        
        return transaction.GetDbTransaction();
    }
}
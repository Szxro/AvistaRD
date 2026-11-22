using AvistaRD.Domain.Common;
using AvistaRD.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace AvistaRD.UnitTests.Infrastructure.Persistence;

// The design-time factory is what `dotnet ef` (and the pipeline's migrations bundle) uses,
// so it must build exactly the same model as the runtime registration.
public class AppDbContextFactoryTests
{
    [Fact]
    public void CreateDbContext_WithoutConnectionString_UsesSqlServerProvider()
    {
        using AppDbContext context = new AppDbContextFactory().CreateDbContext([]);

        Assert.Equal("Microsoft.EntityFrameworkCore.SqlServer", context.Database.ProviderName);
    }

    // Identity tables (AspNetUsers, ...) keep their names because Identity maps them with an
    // explicit ToTable(), which the naming convention doesn't override; the domain tables don't.
    [Fact]
    public void CreateDbContext_AppliesSnakeCaseToDomainTables()
    {
        using AppDbContext context = new AppDbContextFactory().CreateDbContext([]);

        System.Reflection.Assembly domainAssembly = typeof(Entity).Assembly;

        List<string> tablesNotInSnakeCase = context.Model.GetEntityTypes()
            .Where(entityType => entityType.ClrType.Assembly == domainAssembly)
            .Select(entityType => entityType.GetTableName())
            .OfType<string>()
            .Where(tableName => tableName != tableName.ToLowerInvariant())
            .ToList();

        Assert.Empty(tablesNotInSnakeCase);
    }

    [Fact]
    public void CreateDbContext_AppliesSnakeCaseToColumns()
    {
        using AppDbContext context = new AppDbContextFactory().CreateDbContext([]);

        List<string> columnsNotInSnakeCase = context.Model.GetEntityTypes()
            .Where(entityType => entityType.GetTableName() is not null)
            .SelectMany(entityType =>
            {
                StoreObjectIdentifier table = StoreObjectIdentifier.Table(entityType.GetTableName()!, entityType.GetSchema());

                return entityType.GetProperties().Select(property => property.GetColumnName(table));
            })
            .OfType<string>()
            .Where(columnName => columnName != columnName.ToLowerInvariant())
            .ToList();

        Assert.Empty(columnsNotInSnakeCase);
    }
}

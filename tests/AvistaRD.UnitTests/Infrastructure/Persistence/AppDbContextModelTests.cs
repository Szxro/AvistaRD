using AvistaRD.Infrastructure;
using AvistaRD.Infrastructure.Options;
using AvistaRD.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Moq;

namespace AvistaRD.UnitTests.Infrastructure.Persistence;

// Since EF Core 9, Database.Migrate() throws when the model has changes that no migration covers.
// These tests catch that before it reaches the API (Development applies migrations at startup)
// or the pipeline (migrations bundle).
public class AppDbContextModelTests
{
    [Fact]
    public void RuntimeModel_HasNoChangesSinceLastMigration()
    {
        // Same registration the API uses (AddInfrastructureLayer), so Identity and every
        // other runtime option are part of the model being compared.
        ServiceCollection services = new();
        services.AddLogging();
        services.AddSingleton<IConfiguration>(new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?> { ["DatabaseOptions:CommandTimeout"] = "30" })
            .Build());

        Mock<IHostEnvironment> environment = new();
        environment.SetupGet(env => env.EnvironmentName).Returns(Environments.Production);

        services.AddInfrastructureLayer(environment.Object);

        // No database is contacted: the connection string only has to pass validation.
        services.PostConfigure<DatabaseOptions>(options =>
            options.ConnectionString = "Server=localhost;Database=AvistaRD;Trusted_Connection=False;TrustServerCertificate=True");

        using ServiceProvider provider = services.BuildServiceProvider();
        using IServiceScope scope = provider.CreateScope();
        AppDbContext context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        Assert.False(context.Database.HasPendingModelChanges());
    }

    [Fact]
    public void DesignTimeModel_HasNoChangesSinceLastMigration()
    {
        using AppDbContext context = new AppDbContextFactory().CreateDbContext([]);

        Assert.False(context.Database.HasPendingModelChanges());
    }
}

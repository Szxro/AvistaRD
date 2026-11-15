using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace AvistaRD.Infrastructure.Persistence;

public sealed class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    private const string PlaceholderConnection = "Server=localhost;Database=AvistaRD;Trusted_Connection=False;TrustServerCertificate=True";

    public AppDbContext CreateDbContext(string[] args)
    {
        string? fromEnvironment = Environment.GetEnvironmentVariable("DATABASE_CONNECTION");
        string connectionString = string.IsNullOrWhiteSpace(fromEnvironment) ? PlaceholderConnection : fromEnvironment;

        DbContextOptionsBuilder<AppDbContext> optionsBuilder = new();
        optionsBuilder
            .UseSqlServer(connectionString)
            .UseSnakeCaseNamingConvention();

        return new AppDbContext(optionsBuilder.Options);
    }
}
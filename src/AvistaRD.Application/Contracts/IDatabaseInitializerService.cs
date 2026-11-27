namespace AvistaRD.Application.Contracts;

public interface IDatabaseInitializerService
{
    Task CanConnectAsync(CancellationToken cancellationToken = default);

    Task MigrateAsync(CancellationToken cancellationToken = default);

    // Creates the administrator account from ADMIN_EMAIL and ADMIN_PASSWORD when it doesn't exist
    Task SeedAsync(CancellationToken cancellationToken = default);
}
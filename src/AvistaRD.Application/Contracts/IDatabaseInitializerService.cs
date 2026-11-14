namespace AvistaRD.Application.Contracts;

public interface IDatabaseInitializerService
{
    Task CanConnectAsync(CancellationToken cancellationToken = default);

    Task MigrateAsync(CancellationToken cancellationToken = default);
}
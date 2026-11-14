using AvistaRD.Application.Contracts;
using AvistaRD.Infrastructure.Common;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AvistaRD.Infrastructure.Workers;

public class DatabaseInitializerWorker : BaseWorker<DatabaseInitializerWorker>
{
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public DatabaseInitializerWorker(
        ILogger<BaseWorker<DatabaseInitializerWorker>> logger,
        IServiceScopeFactory serviceScopeFactory) : base(logger)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }

    protected override async Task RunAsync(CancellationToken cancellationToken)
    {
        await using AsyncServiceScope scope = _serviceScopeFactory.CreateAsyncScope();

        IDatabaseInitializerService initializer = scope.ServiceProvider.GetRequiredService<IDatabaseInitializerService>();
        
        await initializer.CanConnectAsync(cancellationToken);

        await initializer.MigrateAsync(cancellationToken);
    }
}
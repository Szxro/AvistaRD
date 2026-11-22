using AvistaRD.Application.Contracts;
using AvistaRD.Infrastructure.Common;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AvistaRD.Infrastructure.Workers;

public class DatabaseInitializerWorker : BaseWorker<DatabaseInitializerWorker>
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly IHostEnvironment _environment;

    public DatabaseInitializerWorker(
        ILogger<BaseWorker<DatabaseInitializerWorker>> logger,
        IServiceScopeFactory serviceScopeFactory,
        IHostEnvironment environment) : base(logger)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _environment = environment;
    }

    protected override async Task RunAsync(CancellationToken cancellationToken)
    {
        // Outside Development the deployment pipeline applies the migrations (efbundle),
        // so the API never needs DDL permissions and a slow database can't stop the host.
        if (!_environment.IsDevelopment())
        {
            Logger.LogInformation(
                "Skipping database initialization in {Environment}; migrations are applied by the deployment pipeline",
                _environment.EnvironmentName);

            return;
        }

        await using AsyncServiceScope scope = _serviceScopeFactory.CreateAsyncScope();

        IDatabaseInitializerService initializer = scope.ServiceProvider.GetRequiredService<IDatabaseInitializerService>();

        await initializer.CanConnectAsync(cancellationToken);

        await initializer.MigrateAsync(cancellationToken);
    }
}

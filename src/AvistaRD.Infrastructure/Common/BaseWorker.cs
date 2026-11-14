using AvistaRD.Infrastructure.Common.Attributes;
using AvistaRD.Infrastructure.Common.Enums;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AvistaRD.Infrastructure.Common;

[Inject(ServiceKind.Worker)]
public abstract class BaseWorker<TWorker> : BackgroundService
    where TWorker : class
{
    protected readonly ILogger<BaseWorker<TWorker>> Logger;

    protected BaseWorker(ILogger<BaseWorker<TWorker>> logger)
    {
        Logger = logger;
    }

    public override Task StartAsync(CancellationToken cancellationToken)
    {
        Logger.LogInformation(
            "Worker {workerName} start at {date} with {time}",
            typeof(TWorker).Name,
            DateTime.Now.ToShortDateString(),
            DateTime.Now.ToShortTimeString());

        return base.StartAsync(cancellationToken);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        stoppingToken.Register(() =>
        {
            Logger.LogInformation(
                "Worker {workerName} was signal to stop at {date} with {time}",
                typeof(TWorker).Name,
                DateTime.Now.ToShortDateString(),
                DateTime.Now.ToShortTimeString());
        });

        await RunAsync(stoppingToken);
    }

    protected abstract Task RunAsync(CancellationToken cancellationToken);
}
using AvistaRD.Application.Contracts;
using AvistaRD.Infrastructure.Common;
using AvistaRD.Infrastructure.Workers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace AvistaRD.UnitTests.Infrastructure.Workers;

public class DatabaseInitializerWorkerTests
{
    private readonly Mock<IServiceScopeFactory> _scopeFactory = new();
    private readonly Mock<IDatabaseInitializerService> _initializer = new();

    public DatabaseInitializerWorkerTests()
    {
        Mock<IServiceProvider> serviceProvider = new();
        serviceProvider
            .Setup(provider => provider.GetService(typeof(IDatabaseInitializerService)))
            .Returns(_initializer.Object);

        Mock<IServiceScope> scope = new();
        scope.SetupGet(s => s.ServiceProvider).Returns(serviceProvider.Object);

        _scopeFactory.Setup(factory => factory.CreateScope()).Returns(scope.Object);
    }

    [Theory]
    [InlineData("Production")]
    [InlineData("Staging")]
    public async Task RunAsync_OutsideDevelopment_DoesNotTouchTheDatabase(string environmentName)
    {
        DatabaseInitializerWorker worker = CreateWorker(environmentName);

        await RunToCompletionAsync(worker);

        _scopeFactory.Verify(factory => factory.CreateScope(), Times.Never);
        _initializer.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task RunAsync_InDevelopment_ChecksConnectionAndAppliesMigrations()
    {
        DatabaseInitializerWorker worker = CreateWorker(Environments.Development);

        await RunToCompletionAsync(worker);

        _initializer.Verify(service => service.CanConnectAsync(It.IsAny<CancellationToken>()), Times.Once);
        _initializer.Verify(service => service.MigrateAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    private DatabaseInitializerWorker CreateWorker(string environmentName)
    {
        Mock<IHostEnvironment> environment = new();
        environment.SetupGet(env => env.EnvironmentName).Returns(environmentName);

        return new DatabaseInitializerWorker(
            NullLogger<BaseWorker<DatabaseInitializerWorker>>.Instance,
            _scopeFactory.Object,
            environment.Object);
    }

    private static async Task RunToCompletionAsync(DatabaseInitializerWorker worker)
    {
        await worker.StartAsync(CancellationToken.None);

        Assert.NotNull(worker.ExecuteTask);
        await worker.ExecuteTask;

        await worker.StopAsync(CancellationToken.None);
    }
}

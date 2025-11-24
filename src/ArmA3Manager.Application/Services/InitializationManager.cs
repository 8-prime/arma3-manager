using ArmA3Manager.Application.Common.Interfaces;
using Microsoft.Extensions.Hosting;

namespace ArmA3Manager.Application.Services;

public class InitializationManager : BackgroundService
{
    private readonly IEnumerable<IInitializeable> _initializers;

    public InitializationManager(IEnumerable<IInitializeable> initializers)
    {
        _initializers = initializers;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        foreach (var initializeable in _initializers)
        {
            await initializeable.Initialize();
        }

        Console.WriteLine("Initialization finished");
    }
}
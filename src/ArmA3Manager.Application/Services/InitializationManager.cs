using ArmA3Manager.Application.Common.Enums;
using ArmA3Manager.Application.Common.Interfaces;
using ArmA3Manager.Application.Common.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace ArmA3Manager.Application.Services;

public class InitializationManager : BackgroundService
{
    private readonly IEnumerable<IInitializeable> _initializers;
    private readonly IInitializationInfo _initializationInfo;
    private readonly bool _skipInitialization;

    public InitializationManager(IOptions<ManagerSettings> options, IEnumerable<IInitializeable> initializers,
        IInitializationInfo initializationInfo)
    {
        _skipInitialization = options.Value.SkipInitialization;
        _initializers = initializers;
        _initializationInfo = initializationInfo;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (_skipInitialization)
        {
            Console.WriteLine("Skip Initialization");
            return;
        }

        foreach (var initializeable in _initializers)
        {
            _initializationInfo.SetInitializationResource(initializeable.Name, new InitializationResource
            {
                Name = initializeable.Name,
                Status = InitializationStatus.Created
            });
        }

        foreach (var initializeable in _initializers)
        {
            try
            {
                _initializationInfo.UpdateInitializationResource(initializeable.Name, InitializationStatus.Started);
                await initializeable.Initialize();
                _initializationInfo.UpdateInitializationResource(initializeable.Name, InitializationStatus.Finished);
            }
            catch (Exception ex)
            {
                _initializationInfo.UpdateInitializationResource(initializeable.Name, InitializationStatus.Failed);
                await Console.Error.WriteLineAsync(ex.Message);
            }
        }

        Console.WriteLine("Initialization finished");
    }
}
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Moongate.Core.Data.Events;
using Moongate.Core.Data.Events.Server;
using Moongate.Core.Data.Services;
using Moongate.Core.Interfaces.Services.Base;

namespace Moongate.Core.Interfaces.Services.System;

public class MoongateStartupService : IHostedService
{
    private readonly ILogger<MoongateStartupService> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly IEventBusService _eventBusService;
    private readonly List<ServiceDescriptionData> _servicesToLoad;


    public MoongateStartupService(
        ILogger<MoongateStartupService> logger, IServiceProvider serviceProvider, IEventBusService eventBusService,
        List<ServiceDescriptionData> servicesToLoad
    )
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
        _eventBusService = eventBusService;
        _servicesToLoad = servicesToLoad.OrderBy(s => s.Priority).ToList();
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await _eventBusService.PublishAsync(new ServerStartingEvent(), cancellationToken);
        foreach (var service in _servicesToLoad)
        {
            await LoadServiceAsync(service);
        }

        await _eventBusService.PublishAsync(new ServerStartedEvent(), cancellationToken);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await _eventBusService.PublishAsync(new ServerStoppingEvent(), cancellationToken);

        foreach (var service in _servicesToLoad)
        {
            await LoadServiceAsync(service, true);
        }

        await _eventBusService.PublishAsync(new ServerStoppedEvent(), cancellationToken);
    }

    private async Task LoadServiceAsync(ServiceDescriptionData service, bool isStopping = false)
    {
        try
        {
            var serviceInstance = _serviceProvider.GetRequiredService(service.ServiceType);
            _logger.LogInformation(
                "{TypeOfLoading} service '{ServiceType}' Priority {ServicePriority}",
                isStopping ? "Stopping" : "Starting",
                service.ServiceType.Name,
                service.Priority
            );
            if (serviceInstance is IMoongateStartStopService startStopService)
            {
                if (isStopping)
                {
                    await startStopService.StopAsync();
                }
                else
                {
                    await startStopService.StartAsync();
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading service {ServiceName}", service.ServiceType.Name);
        }
    }
}

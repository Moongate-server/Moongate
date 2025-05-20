using DryIoc;
using Moongate.Core.Data.Events.Server;
using Moongate.Core.Data.Services;
using Moongate.Core.Interfaces.Services.Base;
using Serilog;

namespace Moongate.Core.Interfaces.Services.System;

public class MoongateStartupService
{
    private readonly ILogger _logger = Log.ForContext<MoongateStartupService>();
    private readonly IServiceProvider _serviceProvider;
    private readonly IEventBusService _eventBusService;
    private readonly List<ServiceDescriptionData> _servicesToLoad;


    public MoongateStartupService(
        IContainer serviceProvider, IEventBusService eventBusService,
        List<ServiceDescriptionData> servicesToLoad
    )
    {
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
            var serviceInstance = _serviceProvider.GetService(service.ServiceType);
            _logger.Information(
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
            _logger.Error(ex, "Error loading service {ServiceName}", service.ServiceType.Name);
        }
    }
}

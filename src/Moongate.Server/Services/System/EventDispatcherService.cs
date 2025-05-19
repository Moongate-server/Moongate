using Microsoft.Extensions.Logging;
using Moongate.Core.Extensions.Strings;
using Moongate.Core.Interfaces.Services.System;

namespace Moongate.Server.Services.System;

public class EventDispatcherService : IEventDispatcherService
{
    private readonly Dictionary<string, List<Action<object?>>> _eventHandlers = new();

    private readonly ILogger _logger;

    public EventDispatcherService(ILogger<EventDispatcherService> logger, IEventBusService eventBusService)
    {
        _logger = logger;
        eventBusService.AllEventsObservable.Subscribe(OnEvent);
    }

    private void OnEvent(object obj)
    {
        DispatchEvent(obj.GetType().Name.ToSnakeCase().Replace("_event", ""), obj);
    }


    private void DispatchEvent(string eventName, object? eventData = null)
    {
        _logger.LogDebug("Dispatching event {EventName}", eventName);
        if (!_eventHandlers.TryGetValue(eventName, out var eventHandler))
        {
            return;
        }

        foreach (var handler in eventHandler)
        {
            handler(eventData);
        }
    }

    public void SubscribeToEvent(string eventName, Action<object?> eventHandler)
    {
        if (!_eventHandlers.TryGetValue(eventName, out var eventHandlers))
        {
            eventHandlers = new List<Action<object?>>();
            _eventHandlers.Add(eventName, eventHandlers);
        }

        eventHandlers.Add(eventHandler);
    }

    public void UnsubscribeFromEvent(string eventName, Action<object?> eventHandler)
    {
        if (!_eventHandlers.TryGetValue(eventName, out var eventHandlers))
        {
            return;
        }

        eventHandlers.Remove(eventHandler);
    }
}

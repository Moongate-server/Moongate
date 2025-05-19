using Moongate.Core.Interfaces.EventBus;

namespace Moongate.Core.Data.Internal;

public abstract class EventDispatchJob
{
    public abstract Task ExecuteAsync();
}

/// <summary>
/// Generic implementation of event dispatch job
/// </summary>
public class EventDispatchJob<TEvent> : EventDispatchJob
    where TEvent : class
{
    private readonly IEventBusListener<TEvent> _listener;
    private readonly TEvent _event;


    public EventDispatchJob(IEventBusListener<TEvent> listener, TEvent @event)
    {
        _listener = listener;
        _event = @event;
    }

    public override async Task ExecuteAsync()
    {
        await _listener.HandleAsync(_event);
    }
}

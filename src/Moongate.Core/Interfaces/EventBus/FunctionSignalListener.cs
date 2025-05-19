namespace Moongate.Core.Interfaces.EventBus;

/// <summary>
/// Adapter class that wraps a function to implement IEventBusListener
/// </summary>
public class FunctionSignalListener<TEvent> : IEventBusListener<TEvent>
    where TEvent : class
{
    private readonly Func<TEvent, Task> _handler;

    public FunctionSignalListener(Func<TEvent, Task> handler)
    {
        _handler = handler ?? throw new ArgumentNullException(nameof(handler));
    }

    public Task HandleAsync(TEvent signalEvent, CancellationToken cancellationToken = default)
    {
        return _handler(signalEvent);
    }

    /// <summary>
    /// Checks if this wrapper contains the same handler function
    /// </summary>
    public bool HasSameHandler(Func<TEvent, Task> handler)
    {
        return _handler.Equals(handler);
    }
}

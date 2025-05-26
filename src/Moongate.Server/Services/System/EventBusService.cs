using System.Collections.Concurrent;
using System.Reactive.Subjects;
using System.Threading.Tasks.Dataflow;
using Moongate.Core.Data.Internal;
using Moongate.Core.Interfaces.EventBus;
using Moongate.Core.Interfaces.Services.System;
using Serilog;
using ILogger = Serilog.ILogger;


namespace Moongate.Server.Services.System;

public class EventBusService : IEventBusService
{
    private readonly ILogger _logger = Log.ForContext<EventBusService>();
    private readonly ConcurrentDictionary<Type, object> _listeners = new();
    private readonly ActionBlock<EventDispatchJob> _dispatchBlock;
    private readonly CancellationTokenSource _cts = new();

    private readonly Subject<object> _allEventsSubject = new Subject<object>();

    /// <summary>();
    /// Observable  that emits all events
    /// </summary>
    public IObservable<object> AllEventsObservable => _allEventsSubject;

    public EventBusService()
    {
        var executionOptions = new ExecutionDataflowBlockOptions
        {
            MaxDegreeOfParallelism = 1,
            CancellationToken = _cts.Token
        };

        _dispatchBlock = new ActionBlock<EventDispatchJob>(
            job => job.ExecuteAsync(),
            executionOptions
        );

        _logger.Information(
            "Signal emitter initialized with {ParallelTasks} dispatch tasks",
            1
        );
    }

    /// <summary>
    /// Register a listener for a specific event type
    /// </summary>
    public void Subscribe<TEvent>(IEventBusListener<TEvent> listener) where TEvent : class
    {
        var eventType = typeof(TEvent);

        // Get or create a list of listeners for this event type
        var listeners = (ConcurrentBag<IEventBusListener<TEvent>>)_listeners.GetOrAdd(
            eventType,
            _ => new ConcurrentBag<IEventBusListener<TEvent>>()
        );

        listeners.Add(listener);

        _logger.Verbose(
            "Registered listener {ListenerType} for event {EventType}",
            listener.GetType().Name,
            eventType.Name
        );
    }

    /// <summary>
    /// Register a function as a listener for a specific event type
    /// </summary>
    public void Subscribe<TEvent>(Func<TEvent, Task> handler) where TEvent : class
    {
        var listener = new FunctionSignalListener<TEvent>(handler);
        Subscribe<TEvent>(listener);

        _logger.Verbose(
            "Registered function handler for event {EventType}",
            typeof(TEvent).Name
        );
    }

    /// <summary>
    /// Unregisters a listener for a specific event type
    /// </summary>
    public void Unsubscribe<TEvent>(IEventBusListener<TEvent> listener)
        where TEvent : class
    {
        var eventType = typeof(TEvent);

        if (_listeners.TryGetValue(eventType, out var listenersObj))
        {
            var listeners = (ConcurrentBag<IEventBusListener<TEvent>>)listenersObj;

            // Create a new bag without the listener
            var updatedListeners = new ConcurrentBag<IEventBusListener<TEvent>>(
                listeners.Where(l => !ReferenceEquals(l, listener))
            );

            _listeners.TryUpdate(eventType, updatedListeners, listeners);

            _logger.Verbose(
                "Unregistered listener {ListenerType} from event {EventType}",
                listener.GetType().Name,
                eventType.Name
            );
        }
    }

    /// <summary>
    /// Unregisters a function handler for a specific event type
    /// </summary>
    public void Unsubscribe<TEvent>(Func<TEvent, Task> handler)
        where TEvent : class
    {
        var eventType = typeof(TEvent);

        if (_listeners.TryGetValue(eventType, out var listenersObj))
        {
            var listeners = (ConcurrentBag<IEventBusListener<TEvent>>)listenersObj;

            // Create a new bag without the function handler
            var updatedListeners = new ConcurrentBag<IEventBusListener<TEvent>>(
                listeners.Where(l => !(l is FunctionSignalListener<TEvent> functionListener) ||
                                     !functionListener.HasSameHandler(handler)
                )
            );

            _listeners.TryUpdate(eventType, updatedListeners, listeners);

            _logger.Verbose(
                "Unregistered function handler for event {EventType}",
                eventType.Name
            );
        }
    }

    /// <summary>
    /// Emits an event to all registered listeners asynchronously
    /// </summary>
    public async Task PublishAsync<TEvent>(TEvent eventData, CancellationToken cancellationToken = default)
        where TEvent : class
    {
        var eventType = typeof(TEvent);

        _allEventsSubject.OnNext(eventData);

        if (!_listeners.TryGetValue(eventType, out var listenersObj))
        {
            _logger.Verbose("No listeners registered for event {EventType}", eventType.Name);
            return;
        }

        var listeners = (ConcurrentBag<IEventBusListener<TEvent>>)listenersObj;

        _logger.Verbose(
            "Emitting event {EventType} to {ListenerCount} listeners",
            eventType.Name,
            listeners.Count
        );

        // Dispatch jobs to process the event for each listener
        foreach (var listener in listeners)
        {
            try
            {
                // var job = new EventDispatchJob<TEvent>(listener, eventData);
                //
                // _dispatchBlock.Post(job);
                listener.HandleAsync(eventData, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.Error(
                    ex,
                    "Error dispatching event {EventType} to listener {ListenerType}",
                    eventType.Name,
                    listener.GetType().Name
                );

                throw;
            }
        }
    }

    public int GetListenerCount()
    {
        var totalCount = 0;


        return totalCount;
    }

    public int GetListenerCount<TEvent>() where TEvent : class
    {
        if (_listeners.TryGetValue(typeof(TEvent), out var listenersObj))
        {
            var listeners = (ConcurrentBag<IEventBusListener<TEvent>>)listenersObj;
            return listeners.Count;
        }

        return 0;
    }

    /// <summary>
    /// Waits for all queued events to be processed
    /// </summary>
    public async Task WaitForCompletionAsync()
    {
        _dispatchBlock.Complete();
        await _dispatchBlock.Completion;
    }

    public void Dispose()
    {
        _cts.Cancel();
        _cts.Dispose();
    }
}

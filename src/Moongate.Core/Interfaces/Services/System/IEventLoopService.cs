using Moongate.Core.Data.Metrics.EventLoop;
using Moongate.Core.Interfaces.Services.Base;
using Moongate.Core.Types;

namespace Moongate.Core.Interfaces.Services.System;

/// <summary>
/// Defines a service that manages an event loop for executing actions with different priorities.
/// </summary>
public interface IEventLoopService : IMoongateStartStopService
{
    /// <summary>
    ///  Represents a method that will handle the event loop tick event.
    /// </summary>
    public delegate void EventLoopTickHandler(double tickDurationMs);

    /// <summary>
    ///  Represents a method that will handle the event loop reset event.
    /// </summary>
    public delegate void EventLoopResetHandler();

    /// <summary>
    ///  Occurs when the event loop ticks.
    /// </summary>
    public event EventLoopTickHandler OnTick;

    /// <summary>
    ///  Occurs when the event loop is reset.
    /// </summary>
    public event EventLoopResetHandler OnTickReset;

    /// <summary>
    /// Gets or sets the interval in milliseconds between each tick of the event loop.
    /// </summary>
    int TickIntervalMs { get; set; }

    /// <summary>
    /// Gets the current metrics of the event loop.
    /// </summary>
    EventLoopMetrics Metrics { get; }

    /// <summary>
    /// Enqueues an action to be executed with normal priority.
    /// </summary>
    /// <param name="name">The name of the action for identification.</param>
    /// <param name="action">The action to execute.</param>
    /// <returns>The ID of the queued action.</returns>
    Guid EnqueueAction(string name, Action action);

    /// <summary>
    /// Enqueues an action to be executed with the specified priority.
    /// </summary>
    /// <param name="name">The name of the action for identification.</param>
    /// <param name="action">The action to execute.</param>
    /// <param name="priority">The priority of the action.</param>
    /// <returns>The ID of the queued action.</returns>
    Guid EnqueueAction(string name, Action action, EventLoopPriority priority);

    /// <summary>
    /// Enqueues an action to be executed after the specified delay with normal priority.
    /// </summary>
    /// <param name="name">The name of the action for identification.</param>
    /// <param name="action">The action to execute.</param>
    /// <param name="delay">The delay before executing the action.</param>
    /// <returns>The ID of the queued action.</returns>
    Guid EnqueueDelayedAction(string name, Action action, TimeSpan delay);

    /// <summary>
    /// Enqueues an action to be executed after the specified delay with the specified priority.
    /// </summary>
    /// <param name="name">The name of the action for identification.</param>
    /// <param name="action">The action to execute.</param>
    /// <param name="delay">The delay before executing the action.</param>
    /// <param name="priority">The priority of the action.</param>
    /// <returns>The ID of the queued action.</returns>
    Guid EnqueueDelayedAction(string name, Action action, TimeSpan delay, EventLoopPriority priority);

    /// <summary>
    /// Tries to cancel a previously enqueued action.
    /// </summary>
    /// <param name="actionId">The ID of the action to cancel.</param>
    /// <returns>True if the action was found and canceled; otherwise, false.</returns>
    bool TryCancelAction(Guid actionId);
}

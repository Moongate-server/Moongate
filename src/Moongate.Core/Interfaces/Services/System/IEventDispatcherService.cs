namespace Moongate.Core.Interfaces.Services.System;

public interface IEventDispatcherService
{
    void SubscribeToEvent(string eventName, Action<object?> eventHandler);
    void UnsubscribeFromEvent(string eventName, Action<object?> eventHandler);
}

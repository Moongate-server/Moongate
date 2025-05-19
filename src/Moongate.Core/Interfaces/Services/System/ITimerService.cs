using Moongate.Core.Interfaces.Services.Base;

namespace Moongate.Core.Interfaces.Services.System;

public interface ITimerService : IMoongateStartStopService
{
    string RegisterTimer(string name, double intervalInMs, Action callback, double delayInMs = 0, bool repeat = false);

    void UnregisterTimer(string timerId);

    void UnregisterAllTimers();
}

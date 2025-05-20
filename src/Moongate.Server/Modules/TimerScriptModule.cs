using Moongate.Core.Attributes.Scripts;
using Moongate.Core.Interfaces.Services.System;

namespace Moongate.Server.Modules;

[ScriptModule("timers")]
public class TimerScriptModule
{
    public record TimerRegisterData(
        string Name,
        int IntervalInSeconds,
        Action Callback,
        int DelayInSeconds,
        bool IsRepeat
    );

    private readonly ITimerService _timerService;

    public TimerScriptModule(ITimerService timerService)
    {
        _timerService = timerService;
    }


    [ScriptFunction("add", "Register a timer")]
    public string Register(TimerRegisterData register)
    {
        if (string.IsNullOrEmpty(register.Name))
        {
            throw new ArgumentNullException(nameof(register.Name), "Timer name cannot be null or empty");
        }

        if (register.IntervalInSeconds <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(register.IntervalInSeconds), "Interval must be greater than zero");
        }

        if (register.Callback == null)
        {
            throw new ArgumentNullException(nameof(register.Callback), "Callback cannot be null");
        }

        return _timerService.RegisterTimer(
            register.Name,
            TimeSpan.FromSeconds(register.IntervalInSeconds).TotalMilliseconds,
            register.Callback,
            TimeSpan.FromSeconds(register.DelayInSeconds).TotalMilliseconds,
            register.IsRepeat
        );
    }

    [ScriptFunction("add_repeated", "Register a timer that repeats")]
    public string Repeated(TimerRegisterData repeated)
    {
        return Register(
            repeated with { IsRepeat = true }
        );
    }


    [ScriptFunction("add_once", "Register a timer that runs once")]
    public string OneShot(TimerRegisterData register)
    {
        return Register(
            register with { IsRepeat = false }
        );
    }

    [ScriptFunction("delete", "Unregister a timer")]
    public void Unregister(string timerId)
    {
        if (string.IsNullOrEmpty(timerId))
        {
            throw new ArgumentNullException(nameof(timerId), "Timer ID cannot be null or empty");
        }

        _timerService.UnregisterTimer(timerId);
    }
}

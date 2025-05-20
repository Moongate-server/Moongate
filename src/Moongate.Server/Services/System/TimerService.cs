using System.Collections.Concurrent;
using Microsoft.Extensions.ObjectPool;
using Moongate.Core.Data.Timers;
using Moongate.Core.Interfaces.Services.System;
using Serilog;

namespace Moongate.Server.Services.System;

public class TimerService : ITimerService
{
    private readonly ILogger _logger = Log.ForContext<TimerService>();
    private readonly IEventLoopService _eventLoopService;

    private readonly ObjectPool<TimerDataObject> _timerDataPool = ObjectPool.Create(
        new DefaultPooledObjectPolicy<TimerDataObject>()
    );

    private readonly SemaphoreSlim _timerSemaphore = new(1, 1);
    private readonly BlockingCollection<TimerDataObject> _timers = new();

    public TimerService(IEventLoopService eventLoopService)
    {
        _eventLoopService = eventLoopService;
    }

    private void EventLoopServiceOnOnTick(double tickDurationMs)
    {
        _timerSemaphore.Wait();

        foreach (var timer in _timers)
        {
            timer.DecrementRemainingTime(tickDurationMs);

            if (timer.RemainingTimeInMs <= 0)
            {
                try
                {
                    timer.Callback?.Invoke();
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "Error executing timer callback for {TimerId}", timer.Id);
                }

                if (timer.Repeat)
                {
                    timer.ResetRemainingTime();
                }
                else
                {
                    _timers.TryTake(out var _);
                    _logger.Information("Unregistering timer: {TimerId}", timer.Id);
                }
            }
        }

        _timerSemaphore.Release();
    }

    public Task StartAsync(CancellationToken cancellationToken = default)
    {
        _eventLoopService.OnTick += EventLoopServiceOnOnTick;
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public string RegisterTimer(string name, double intervalInMs, Action callback, double delayInMs = 0, bool repeat = false)
    {
        var existingTimer = _timers.FirstOrDefault(t => t.Name == name);

        if (existingTimer != null)
        {
            _logger.Warning("Timer with name {Name} already exists. Unregistering it.", name);
            UnregisterTimer(existingTimer.Id);
        }

        _timerSemaphore.Wait();

        var timerId = Guid.NewGuid().ToString();
        var timer = _timerDataPool.Get();

        timer.Name = name;
        timer.Id = timerId;
        timer.IntervalInMs = intervalInMs;
        timer.Callback = callback;
        timer.Repeat = repeat;
        timer.RemainingTimeInMs = intervalInMs;
        timer.DelayInMs = delayInMs;


        _timers.Add(timer);

        _timerSemaphore.Release();

        _logger.Debug(
            "Registering timer: {TimerId}, Interval: {IntervalInSeconds} ms, Repeat: {Repeat}",
            timerId,
            intervalInMs,
            repeat
        );

        return timerId;
    }

    public void UnregisterTimer(string timerId)
    {
        _timerSemaphore.Wait();

        var timer = _timers.FirstOrDefault(t => t.Id == timerId);

        if (timer != null)
        {
            _timers.TryTake(out timer);
            _logger.Information("Unregistering timer: {TimerId}", timer.Id);
            _timerDataPool.Return(timer);
        }
        else
        {
            _logger.Warning("Timer with ID {TimerId} not found", timerId);
        }

        _timerSemaphore.Release();
    }

    public void UnregisterAllTimers()
    {
        _timerSemaphore.Wait();

        while (_timers.TryTake(out var timer))
        {
            _logger.Information("Unregistering timer: {TimerId}", timer.Id);
        }

        _timerSemaphore.Release();
    }

    public void Dispose()
    {
        _timerSemaphore.Dispose();
        _timers.Dispose();

        _eventLoopService.OnTick -= EventLoopServiceOnOnTick;

        GC.SuppressFinalize(this);
    }
}

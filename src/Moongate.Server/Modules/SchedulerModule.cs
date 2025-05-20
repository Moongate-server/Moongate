using Moongate.Core.Attributes.Scripts;
using Moongate.Core.Interfaces.Services.System;

namespace Moongate.Server.Modules;

[ScriptModule("scheduler")]
public class SchedulerModule
{
    private readonly ISchedulerSystemService _schedulerSystemService;

    public SchedulerModule(ISchedulerSystemService schedulerSystemService)
    {
        _schedulerSystemService = schedulerSystemService;
    }

    [ScriptFunction("add","Schedule a task to be run every x seconds")]
    public void ScheduleTask(string name, int seconds, Action callback)
    {
        _schedulerSystemService.RegisterJob(
            name,
            () =>
            {
                callback();
                return Task.CompletedTask;
            },
            TimeSpan.FromSeconds(seconds)
        );
    }
}

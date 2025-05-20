using Moongate.Core.Interfaces.Services.Base;

namespace Moongate.Core.Interfaces.Services.System;

public interface ISchedulerSystemService : IMoongateService
{
    Task RegisterJob(string name, Func<Task> task, TimeSpan interval);
    Task UnregisterJob(string name);
    Task<bool> IsJobRegistered(string name);
    Task PauseJob(string name);
    Task ResumeJob(string name);
}

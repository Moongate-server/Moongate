

namespace Moongate.Core.Data.Events.Scheduler;

public abstract record AddSchedulerJobEvent(string Name, TimeSpan TotalSpan, Func<Task> Action);

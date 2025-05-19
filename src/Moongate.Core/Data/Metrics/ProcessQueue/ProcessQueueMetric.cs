namespace Orion.Core.Server.Data.Metrics.ProcessQueue;

public record ProcessQueueMetric(
    string Context,
    int QueuedItems,
    int ProcessedItems,
    int FailedItems,
    TimeSpan AverageProcessingTime
);

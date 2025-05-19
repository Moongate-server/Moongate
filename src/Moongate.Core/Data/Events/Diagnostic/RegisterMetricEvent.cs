using Moongate.Core.Interfaces.Metrics;

namespace Moongate.Core.Data.Events.Diagnostic;

public record RegisterMetricEvent(IMetricsProvider provider);

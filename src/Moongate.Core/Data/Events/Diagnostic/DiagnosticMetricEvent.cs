using Moongate.Core.Data.Metrics.Diagnostic;

namespace Moongate.Core.Data.Events.Diagnostic;

public record DiagnosticMetricEvent(MetricProviderData Metrics);

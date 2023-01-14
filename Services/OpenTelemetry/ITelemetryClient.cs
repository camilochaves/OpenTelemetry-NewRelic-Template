using System.Diagnostics;

namespace opentelemetry_newrelic_template.Services.OpenTelemetry;
public interface ITelemetryClient
{
    Activity? Report(string name, TracerTelemetrySource source = TracerTelemetrySource.Generic);
}

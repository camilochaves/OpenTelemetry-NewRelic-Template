using System.Diagnostics;
using opentelemetry_newrelic_template.Services.OpenTelemetry;

namespace opentelemetry_newrelic_template.Patterns;

public class ProcessGenericTelemetrySource : TestCondition<TracerTelemetrySource, Activity>
{
    public ProcessGenericTelemetrySource(TracerTelemetrySource input) : base(input)
    {
    }

    public override bool Rule() => _input == TracerTelemetrySource.Generic;

    public override Activity? Handle(params object[] args)
    {
        if (args.Length < 1) return null;
        var name = args[0] as string;
        return TelemetryClient.Generic.StartActivity(name: name);
    }
}

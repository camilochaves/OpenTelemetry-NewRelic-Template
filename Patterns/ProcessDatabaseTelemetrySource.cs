using System.Diagnostics;
using opentelemetry_newrelic_template.Services.OpenTelemetry;

namespace opentelemetry_newrelic_template.Patterns;
public class ProcessDatabaseTelemetrySource : TestCondition<TracerTelemetrySource, Activity>
{
    public ProcessDatabaseTelemetrySource(TracerTelemetrySource input) : base(input)
    {
    }

    public override bool Rule() => _input == TracerTelemetrySource.Database;

    public override Activity? Handle(params object[] args)
    {
        if (args.Length == 0) return null;
        var name = args[0] as string;

        return TelemetryClient.Database.StartActivity(
                name: name,
                kind: ActivityKind.Client,
                parentId: null!,
                tags: new List<KeyValuePair<string, object>>()
                {
                        new("db.system", "CustomDatabaseName")
                }!
           );
    }
}

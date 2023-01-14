using System.Diagnostics;
using opentelemetry_newrelic_template.Patterns;

namespace opentelemetry_newrelic_template.Services.OpenTelemetry;
public class TelemetryClient : ITelemetryClient
{
    public static readonly ActivitySource Generic = new(nameof(TracerTelemetrySource.Generic));
    public static readonly ActivitySource Database = new(nameof(TracerTelemetrySource.Database));

    public TelemetryClient() { }

    public Activity? Report(string name, TracerTelemetrySource source = TracerTelemetrySource.Generic)
    {
        //This is the Rule Design Pattern to avoid IFs
        IEnumerable<TestCondition<TracerTelemetrySource, Activity>> trueConditions =
            new CheckIfRuleFor<TracerTelemetrySource, Activity>(source)
                .AddCheckCondition(typeof(ProcessGenericTelemetrySource))
                .AddCheckCondition(typeof(ProcessDatabaseTelemetrySource))
                .Build();

        var activity = trueConditions.First(c => c.Rule())?.Handle(name);

        activity?.AddTag("MyCustomKeyAttribute", "Value");

        return activity;
    }
}

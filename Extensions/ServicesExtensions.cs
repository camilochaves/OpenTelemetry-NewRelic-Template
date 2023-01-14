using System.Reflection;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace opentelemetry_newrelic_template.Extensions;

public static class ServicesExtensions
{
    public static IServiceCollection ConfigureOpenTelemetry(this IServiceCollection services
        , IConfiguration configuration)
    {
        var oTelEndpoint = configuration.GetValue<string>("OpenTelemetry:Exporters:Otlp:Endpoint");
        var serviceName = configuration.GetValue<string>("ServiceName");
        AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

        var resourceBuilder = ResourceBuilder.CreateDefault();

        void ConfigureResource(ref ResourceBuilder r) => r.AddService(
               serviceName: serviceName,
               serviceVersion: Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "unknown",
               serviceInstanceId: Environment.MachineName);

        ConfigureResource(ref resourceBuilder);

        resourceBuilder.AddTelemetrySdk();

        if (configuration.GetValue<bool>("OpenTelemetry:EnableTracing"))
        {
            services.AddOpenTelemetry().WithTracing(builder =>
                {
                    builder
                        //.SetSampler(new AlwaysOnSampler())
                        .SetResourceBuilder(resourceBuilder)
                        .AddAspNetCoreInstrumentation(options =>
                        {
                            options.RecordException = true;
                        })
                        .AddHttpClientInstrumentation()
                        .AddSource("Generic")
                        .AddSource("Database")
                        .AddOtlpExporter(exporterOptions =>
                        {
                            exporterOptions.Endpoint = new Uri(oTelEndpoint);
                            exporterOptions.ExportProcessorType = ExportProcessorType.Simple;
                        })
                        .AddConsoleExporter();
                })
            .StartWithHost();
        }

        if (configuration.GetValue<bool>("OpenTelemetry:EnableMetrics"))
        {
            services.AddOpenTelemetry().WithMetrics(builder =>
            {
                builder
                    .SetResourceBuilder(resourceBuilder)
                    .AddRuntimeInstrumentation()
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddOtlpExporter((exporterOptions, metricReaderOptions) =>
                        {
                            exporterOptions.Endpoint = new Uri(oTelEndpoint);
                            exporterOptions.ExportProcessorType = ExportProcessorType.Batch;
                            metricReaderOptions.TemporalityPreference = MetricReaderTemporalityPreference.Delta;
                        });
            });
        }

        return services;
    }


}
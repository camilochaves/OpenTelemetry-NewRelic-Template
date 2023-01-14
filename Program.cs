using OpenTelemetry.Resources;
using System.Reflection;
using OpenTelemetry;
using OpenTelemetry.Logs;
using opentelemetry_newrelic_template.Extensions;
using opentelemetry_newrelic_template.Services.OpenTelemetry;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;
var isLoggingEnabled = configuration.GetValue<bool>("OpenTelemetry:EnableLogging");


if (isLoggingEnabled)
{
    var oTelEndpoint = configuration.GetValue<string>("OpenTelemetry:Exporters:Otlp:Endpoint");
    var serviceName = configuration.GetValue<string>("ServiceName");

    void ConfigureResource(ref ResourceBuilder r) => r.AddService(
        serviceName: serviceName!,
        serviceVersion: Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "unknown",
        serviceInstanceId: Environment.MachineName);
    var resourceBuilder = ResourceBuilder.CreateDefault();
    ConfigureResource(ref resourceBuilder);

    resourceBuilder.AddAttributes(new List<KeyValuePair<string, object>>()
                {
                    new KeyValuePair<string, object>("myKey","myValue")
                });

    builder.Services
        .AddLogging(logging => logging
            .ClearProviders()
            .AddConsole()
            .AddOpenTelemetry(options =>
            {
                options.IncludeFormattedMessage = true;
                options.IncludeScopes = true;
                options.ParseStateValues = true;

                options
                    .SetResourceBuilder(resourceBuilder)
                    .AddOtlpExporter(exporterOptions =>
                    {
                        exporterOptions.Endpoint = new Uri(oTelEndpoint);
                        exporterOptions.ExportProcessorType = ExportProcessorType.Simple;
                    });
            }));

}
else
        builder.Services.AddLogging(logging => logging.ClearProviders());


builder.Services.ConfigureOpenTelemetry(configuration);
builder.Services.AddTransient<ITelemetryClient, TelemetryClient>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();





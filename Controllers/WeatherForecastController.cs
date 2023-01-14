using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using opentelemetry_newrelic_template.Extensions;
using opentelemetry_newrelic_template.Services.OpenTelemetry;

namespace opentelemetry_newrelic_template.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<WeatherForecastController> _logger;
    private readonly ITelemetryClient _telemetry;

    public WeatherForecastController(ILogger<WeatherForecastController> logger, ITelemetryClient telemetry)
    {
        _logger = logger;
        _telemetry = telemetry;
    }

    [HttpGet]
    [Route("[action]/{city}")]
    public async Task<IEnumerable<WeatherForecast>> GetForecastFrom([FromRoute] string city)
    {
        _logger.LogInformation($"Got forecast from {city}");
        using var activity = _telemetry.Report(MethodBase.GetCurrentMethod()?.GetNameOfAsyncMethod(), TracerTelemetrySource.Generic);


        //Simulate Call To Database
        await Task.Delay(200);
        var databaseActivityMetric = _telemetry.Report("Database Access", TracerTelemetrySource.Database);
        var result = Enumerable.Range(1, 5).Select(index => new WeatherForecast
        {
            Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        })
        .ToArray();
        databaseActivityMetric?.Dispose();
        
        return result;
    }
}

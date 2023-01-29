using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using StackExchange.Redis;

namespace Protobuff.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private IConnectionMultiplexer _connectionMultiplexer;
    
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<WeatherForecastController> _logger;
    private readonly IDatabase _db; 

    public WeatherForecastController(ILogger<WeatherForecastController> logger, IConnectionMultiplexer connectionMultiplexer)
    {
        _logger = logger;
        _connectionMultiplexer = connectionMultiplexer;
        _db = _connectionMultiplexer.GetDatabase();
    }

    [HttpGet(Name = "GetWeatherForecast")]
    public async Task<IEnumerable<WeatherForecast>?> Get()
    {

        WeatherForecast[]? result = null;
        var key = "weather_cost";
        
        var cashed = await _db.StringGetAsync(key);

        if (string.IsNullOrEmpty(cashed))
        {
            result =  Enumerable.Range(1, 5).Select(index => new WeatherForecast
                {
                    Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    TemperatureC = Random.Shared.Next(-20, 55),
                    Summary = Summaries[Random.Shared.Next(Summaries.Length)]
                })
                .ToArray();
            await _db.StringSetAsync(key, JsonSerializer.Serialize(result));
        }
        else
        {
            result = JsonSerializer.Deserialize<WeatherForecast[]>(cashed!);
        }

        return result;
    }
}
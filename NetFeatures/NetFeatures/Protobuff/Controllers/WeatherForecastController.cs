using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using ProtoBuf;
using Protobuff.Services;
using StackExchange.Redis;

namespace Protobuff.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
  
    private readonly ILogger<WeatherForecastController> _logger;
    private readonly IWeatherCastService _weatherCastService;
    private readonly IDatabase _db; 
    

    public WeatherForecastController(ILogger<WeatherForecastController> logger, IWeatherCastService weatherCastService)
    {
        _logger = logger;
        _weatherCastService = weatherCastService;
    }

    [HttpGet("~/GetWithJsonSerilazer")]
    public async Task<IEnumerable<WeatherForecast>?> GetWithJsonSerilazer()
    {
        return await _weatherCastService.WithJsonSerilazer();
    }
    
    [HttpGet("~/GetWithProtoSerilazer")]
    public async Task<IEnumerable<WeatherForecast>?> GetWithProtoSerilazer()
    {
        return await _weatherCastService.WithProtoSerilazer();
    }

   
}
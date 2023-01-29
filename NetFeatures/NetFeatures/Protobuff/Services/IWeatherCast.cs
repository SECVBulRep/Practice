using System.Text.Json;
using ProtoBuf;
using StackExchange.Redis;

namespace Protobuff.Services;

public interface IWeatherCastService
{
    Task<IEnumerable<WeatherForecast>> WithJsonSerilazer();

    Task<IEnumerable<WeatherForecast>> WithProtoSerilazer();
}


public class WeatherCastService : IWeatherCastService
{

    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };
    
    public readonly IConnectionMultiplexer _connectionMultiplexer;
    private readonly IDatabase _db;

    public WeatherCastService(IConnectionMultiplexer connectionMultiplexer)
    {
        _connectionMultiplexer = connectionMultiplexer;
        _db = _connectionMultiplexer.GetDatabase();
    }

    public async Task<IEnumerable<WeatherForecast>> WithJsonSerilazer()
    {
        WeatherForecast[]? result = null;
        var key = "weather_costWithJsonSerilazer";

        var cashed = await _db.StringGetAsync(key);

        if (string.IsNullOrEmpty(cashed))
        {
            var temperature = Random.Shared.Next(-20, 55);
            result =  Enumerable.Range(1, 5).Select(index => new WeatherForecast
                {
                    Date = DateTime.Now.AddDays(index),
                    TemperatureC = temperature,
                    Summary = Summaries[Random.Shared.Next(Summaries.Length)],
                    TemperatureF = 32 + (int) (temperature / 0.5556)
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

    public async Task<IEnumerable<WeatherForecast>> WithProtoSerilazer()
    {
        WeatherForecast[]? result = null;
        var key = "weather_costWithProtoSerilazer";
        
        var cashed = await _db.StringGetAsync(key);

        if (string.IsNullOrEmpty(cashed))
        {
            var temperature = Random.Shared.Next(-20, 55);
            result =  Enumerable.Range(1, 5).Select(index => new WeatherForecast
                {
                    Date = DateTime.Now.AddDays(index),
                    TemperatureC = temperature,
                    Summary = Summaries[Random.Shared.Next(Summaries.Length)],
                    TemperatureF = 32 + (int) (temperature / 0.5556)
                })
                .ToArray();
            await _db.StringSetAsync(key, ProtoSerializer<WeatherForecast[]>(result));
        }
        else
        {
            result = Serializer.Deserialize<WeatherForecast[]>(cashed!);
        }

        return result;
    }


    public void Proto()
    {
        var temperature = Random.Shared.Next(-20, 55);
        var result =  Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = temperature,
                Summary = Summaries[Random.Shared.Next(Summaries.Length)],
                TemperatureF = 32 + (int) (temperature / 0.5556)
            })
            .ToArray();

       var ser =  ProtoSerializer<WeatherForecast[]>(result);
       Serializer.Deserialize<WeatherForecast[]>((ReadOnlyMemory<byte>) ser);
    }

    
    public void Json()
    {
        var temperature = Random.Shared.Next(-20, 55);
        var result =  Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = temperature,
                Summary = Summaries[Random.Shared.Next(Summaries.Length)],
                TemperatureF = 32 + (int) (temperature / 0.5556)
            })
            .ToArray();

        var ser = JsonSerializer.Serialize(result);
        JsonSerializer.Deserialize<WeatherForecast[]>(ser);
    }
    

    private static byte[] ProtoSerializer<T>(T record) where T : class
    {
        using (var stream = new MemoryStream())
        {
            Serializer.Serialize(stream,record);
            return stream.ToArray();
        }
    }
}
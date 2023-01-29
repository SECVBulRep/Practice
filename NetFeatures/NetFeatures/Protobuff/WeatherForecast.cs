using ProtoBuf;

namespace Protobuff;


[ProtoContract]
public class WeatherForecast
{
    [ProtoMember(1)]
    public DateTime Date { get; set; }

    [ProtoMember(2)]
    public int TemperatureC { get; set; }

    [ProtoMember(3)]
    public int TemperatureF { get; set; } 

    [ProtoMember(4)]
    public string? Summary { get; set; }

    [ProtoMember(5)]
    public WeatherForecast Forecast { get; set; }
}
using BenchmarkDotNet.Attributes;
using Protobuff.Services;
using StackExchange.Redis;

namespace Test;


[MemoryDiagnoser()]
public class Benchy
{
    private ConnectionMultiplexer cm;

    private WeatherCastService _castService;
    
    [GlobalSetup]
    public void GlobalSetup()
    {
        cm = ConnectionMultiplexer.Connect("localhost:6379");
        var db = cm.GetDatabase();
        _castService = new WeatherCastService(cm);
    }

    [Benchmark]
    public async Task WithJsonSerilazer()
    {
        _castService.Json();
    }
    [Benchmark]
    public async Task WithProtoSerilazer()
    {
        _castService.Proto();
    }
}
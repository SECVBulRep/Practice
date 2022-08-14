// See https://aka.ms/new-console-template for more information

using Confluent.Kafka;
using Newtonsoft.Json;

var config = new ProducerConfig
{
    BootstrapServers = "localhost:9092",
};

using var producer = new ProducerBuilder<Null, string>(config).Build();

try
{
    string? state;
    while ((state = Console.ReadLine()) != null)
    {
        var response = producer.ProduceAsync("weather-topic",
            new Message<Null, string> {Value = JsonConvert.SerializeObject(new Weather(state, 70))});
    }
}
catch (ProduceException<Null, string> exc)
{
    Console.WriteLine(exc);
}
catch (Exception e)
{
    Console.WriteLine(e);
}

public record Weather(string State, int Temperature);
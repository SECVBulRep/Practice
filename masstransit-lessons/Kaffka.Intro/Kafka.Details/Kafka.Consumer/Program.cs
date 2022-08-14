using System.Text.Json.Serialization;
using Confluent.Kafka;
using Newtonsoft.Json;

var config = new ConsumerConfig
{
    GroupId = "weather-consumer-group",
    BootstrapServers = "localhost:9092",
    AutoOffsetReset = AutoOffsetReset.Earliest,
    EnableAutoCommit = false,
    
    
    
};

var config2 = new ConsumerConfig
{
    GroupId = "weather-consumer-group2",
    BootstrapServers = "localhost:9092",
    AutoOffsetReset = AutoOffsetReset.Earliest
};

Task.Run(() =>
{
    using var consumer = new ConsumerBuilder<Null, string>(config).Build();
    consumer.Subscribe("weather-topic");
    CancellationTokenSource token = new();
    try
    {
        while (true)
        {
            var response = consumer.Consume(token.Token);

            if (response.Message != null)
            {
                var weather = JsonConvert.DeserializeObject<Weather>(response.Message.Value);

                Console.WriteLine($" Consumer1: {weather.State} {weather.Temperature}");
                consumer.Commit(response);
            }
        }
    }
    catch (Exception exception)
    {
        Console.WriteLine(exception);
    }
});

Task.Run(() =>
{
    using var consumer2 = new ConsumerBuilder<Null, string>(config).Build();
    consumer2.Subscribe("weather-topic");


    CancellationTokenSource token = new();

    try
    {
        while (true)
        {
            var response = consumer2.Consume(token.Token);

            if (response.Message != null)
            {
                var weather = JsonConvert.DeserializeObject<Weather>(response.Message.Value);
                Console.WriteLine($" Consumer2: {weather.State} {weather.Temperature}");
               // consumer2.Commit(response);
            }
        }
    }
    catch (Exception exception)
    {
        Console.WriteLine(exception);
    }
});


Task.Run(() =>
{
    using var consumer3 = new ConsumerBuilder<Null, string>(config2).Build();
    consumer3.Subscribe("weather-topic");


    CancellationTokenSource token = new();

    try
    {
        while (true)
        {
            var response = consumer3.Consume(token.Token);

            if (response.Message != null)
            {
                var weather = JsonConvert.DeserializeObject<Weather>(response.Message.Value);
                Console.WriteLine($" Consumer3: {weather.State} {weather.Temperature}");
            }
        }
    }
    catch (Exception exception)
    {
        Console.WriteLine(exception);
    }
});



while (Console.ReadLine() != null)
{
    
}

public record Weather(string State, int Temperature);
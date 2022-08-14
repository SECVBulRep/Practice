// See https://aka.ms/new-console-template for more information

using Confluent.Kafka;

var config = new ProducerConfig
{
    BootstrapServers = "localhost:9092",
};

using var producer = new ProducerBuilder<Null, string>(config).Build();

try
{
    var response = producer.ProduceAsync("weather-topic", new Message<Null, string> { Value ="Hello Kafka!!"});
}
catch (Exception e)
{
    Console.WriteLine(e);
}

Console.WriteLine("Hello, World!");



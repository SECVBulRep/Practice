// See https://aka.ms/new-console-template for more information


using System.Net;
using Confluent.Kafka;
using Confluent.Kafka.Examples.AvroSpecific;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;


internal class Program
{
    static HttpClient _client;

    static async Task Main(string[] args)
    {
        var services = new ServiceCollection();
        ConfigureServices(services);
        await using var provider = services.BuildServiceProvider(true);

        await Task.Run(() => Client(provider), CancellationToken.None);
    }


    private static void ConfigureServices(IServiceCollection services)
    {
        services.AddLogging(configure => configure.AddConsole());
    }

    static async Task Client(IServiceProvider provider)
    {
        var logger = provider.GetRequiredService<ILogger<Program>>();

        var config = new ProducerConfig
        {
            BootstrapServers = "localhost:9092",
            ClientId = Dns.GetHostName(),
        };

        var schemaRegistryConfig = new SchemaRegistryConfig
        {
            Url = "localhost:8081"
        };

        var jsonSerializerConfig = new JsonSerializerConfig
        {
            BufferBytes = 100
        };

        var avroSerializerConfig = new AvroSerializerConfig
        {
            // optional Avro serializer properties:
            BufferBytes = 100
        };



        while (true)
        {
            Console.Write("Enter # of courier to visit, or empty to quit: ");

            var line = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(line))
                break;


            int loops = Convert.ToInt32(line);
            string topic = nameof(ICurrierEntered);

            using (var schemaRegistry = new CachedSchemaRegistryClient(schemaRegistryConfig))
            using (var producer =
                   new ProducerBuilder<Null, ICurrierEntered>(config)
                       .SetValueSerializer(new AvroSerializer<ICurrierEntered>(schemaRegistry, avroSerializerConfig))
                       .Build())
            {
                for (var pass = 0; pass < loops; pass++)
                {
                    ICurrierEntered user = new ICurrierEntered { Timestamp = DateTime.Now.ToString(), CurrierId = Guid.NewGuid().ToString() };
                    var res = await producer
                        .ProduceAsync(topic, new Message<Null, ICurrierEntered> { Key = null, Value = user });
                }
            }
        }
    }
}
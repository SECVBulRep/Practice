// See https://aka.ms/new-console-template for more information


using System.Net;
using confluent.io.examples.serialization.avro;
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
        var config = new ProducerConfig
        {
            BootstrapServers = "localhost:9092",
            ClientId = Dns.GetHostName(),
        };

        var schemaRegistryConfig = new SchemaRegistryConfig
        {
            Url = "localhost:8081"
        };

        var avroSerializerConfig = new AvroSerializerConfig
        {
            BufferBytes = 100
        };

        while (true)
        {
            Console.Write("Enter # of courier to visit, or empty to quit: ");

            var line = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(line))
                break;


            var random = new Random();
            
            int loops = Convert.ToInt32(line);
            string topic_ICurrierEntered = nameof(ICurrierEntered);
            string topic_ICurrierLeft = nameof(ICurrierLeft);

            using (var schemaRegistry = new CachedSchemaRegistryClient(schemaRegistryConfig))
            using (var producer_ICurrierEntered =
                   new ProducerBuilder<Null, ICurrierEntered>(config)
                       .SetValueSerializer(new AvroSerializer<ICurrierEntered>(schemaRegistry, avroSerializerConfig))
                       .Build())
            using (var producer_ICurrierLeft =
                   new ProducerBuilder<Null, ICurrierLeft>(config)
                       .SetValueSerializer(new AvroSerializer<ICurrierLeft>(schemaRegistry, avroSerializerConfig))
                       .Build())
            {
                var taskList = new List<Task>();
                
                for (var pass = 0; pass < loops; pass++)
                {
                    var curierId = Guid.NewGuid();
                    var timestamp = DateTime.Now;
                    
                    ICurrierEntered currierEntered = new ICurrierEntered { Timestamp = timestamp.ToString(), CurrierId = curierId.ToString() };
                   
                    var taskEnter =  producer_ICurrierEntered
                        .ProduceAsync(topic_ICurrierEntered, new Message<Null, ICurrierEntered> { Key = null, Value = currierEntered });
                    
                    ICurrierLeft currierLeft = new ICurrierLeft { Timestamp = timestamp.AddSeconds(random.Next(100)).ToString(), CurrierId = curierId.ToString() };
                    
                    var taskLeft =  producer_ICurrierLeft
                        .ProduceAsync(topic_ICurrierLeft, new Message<Null, ICurrierLeft> { Key = null, Value = currierLeft });
                    
                    taskList.Add(taskEnter);
                    taskList.Add(taskLeft);
                }

              await  Task.WhenAll(taskList);
            }
        }
    }
}
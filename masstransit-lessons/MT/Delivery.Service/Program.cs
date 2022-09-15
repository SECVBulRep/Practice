using System.Data;
using System.Reflection;
using confluent.io.examples.serialization.avro;
using Confluent.Kafka;
using Confluent.Kafka.Examples.AvroSpecific;
using Confluent.Kafka.SyncOverAsync;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using Delivery.Components.Consumers;
using Delivery.Components.StateMachines;
using Delivery.Contracts;
using Delivery.Service;
using MassTransit;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;


await Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((hostingConext, config) => { config.AddJsonFile("appsettings.json", true); })
    .ConfigureServices((hostContext, services) =>
    {
        services.TryAddSingleton(KebabCaseEndpointNameFormatter
            .Instance); 

        services.AddMassTransit(cfg =>
        {
            cfg.SetKebabCaseEndpointNameFormatter();
            cfg.AddConsumer<DeliverOrderConsumer>(typeof(DeliverOrderConsumerDefitnition));
            cfg.AddConsumer<CurrierVisitedConsumer>();

            
            var schemaRegistryClient = new CachedSchemaRegistryClient(new Dictionary<string, string>
            {
                {"schema.registry.url", "localhost:8081"},
            });
            
            

            cfg.AddSagaStateMachine<OrderDeliveryStateMachine, OrderDeliveryState, OrderDeliverySagaDefinition>()
                .EntityFrameworkRepository(r =>
                {
                    r.ConcurrencyMode = ConcurrencyMode.Pessimistic;

                    r.AddDbContext<DbContext, OrderDeliveryStateDbContext>((provider, optionsBuilder) =>
                    {
                        optionsBuilder.UseSqlServer(hostContext.Configuration.GetConnectionString("service"), m =>
                        {
                            m.MigrationsAssembly(Assembly.GetExecutingAssembly().GetName().Name);
                            m.MigrationsHistoryTable($"__{nameof(OrderDeliveryStateDbContext)}");
                        });
                    });
                });


            cfg.AddRider(rider =>
            {
                rider.AddSagaStateMachine<СurrierStateMachine, СurrierState, СurrierStateDefinition>()
                    .InMemoryRepository();

                rider.AddProducer<ICurrierVisited>(nameof(ICurrierVisited));

                rider.UsingKafka((context, k) =>
                {
                    k.Host("localhost:9092");
                    
                    k.TopicEndpoint<string, ICurrierEntered>(nameof(ICurrierEntered),  nameof(Assembly.GetName), c =>
                    {
                        c.SetKeyDeserializer(new AvroDeserializer<string>(schemaRegistryClient).AsSyncOverAsync());
                        c.SetValueDeserializer(new AvroDeserializer<ICurrierEntered>(schemaRegistryClient).AsSyncOverAsync());
                        c.AutoOffsetReset = AutoOffsetReset.Earliest;
                        c.ConfigureSaga<СurrierState>(context);
                        c.CreateIfMissing(m =>
                        {
                            m.NumPartitions = 1;
                        });
                    });
                    
                    k.TopicEndpoint<string, ICurrierLeft>(nameof(ICurrierLeft),  nameof(Assembly.GetName), c =>
                    {
                        c.SetKeyDeserializer(new AvroDeserializer<string>(schemaRegistryClient).AsSyncOverAsync());
                        c.SetValueDeserializer(new AvroDeserializer<ICurrierLeft>(schemaRegistryClient).AsSyncOverAsync());
                        c.AutoOffsetReset = AutoOffsetReset.Earliest;
                        c.ConfigureSaga<СurrierState>(context);
                        c.CreateIfMissing(m =>
                        {
                            m.NumPartitions = 1;
                        });
                    });
                });
            });
            
            cfg.UsingRabbitMq((context, config) =>
            {
                config.Host("localhost", "work", h =>
                {
                    h.Username("guest");
                    h.Password("guest");
                });

                config.ConfigureEndpoints(context);
            });
        });
    })
    .ConfigureLogging((hostingContext, logging) =>
    {
        logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
        logging.AddConsole(x => { });
    })
    .Build()
    .RunAsync();


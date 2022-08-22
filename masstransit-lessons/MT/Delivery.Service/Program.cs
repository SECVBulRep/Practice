using System.Data;
using System.Reflection;
using Confluent.Kafka;
using Delivery.Components.Consumers;
using Delivery.Components.StateMachines;
using Delivery.Contracts;
using Delivery.Service;
using MassTransit;
using MassTransit.Courier.Contracts;
using MassTransit.MongoDbIntegration.MessageData;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using StackExchange.Redis;

await Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((hostingConext, config) => { config.AddJsonFile("appsettings.json", true); })
    .ConfigureServices((hostContext, services) =>
    {
        services.TryAddSingleton(KebabCaseEndpointNameFormatter
            .Instance); // позже напиши. kebab case лучше чем snake _ kase

        services.AddMassTransit(cfg =>
        {
            cfg.SetKebabCaseEndpointNameFormatter();
            cfg.AddConsumer<DeliverOrderConsumer>(typeof(DeliverOrderConsumerDefitnition));
            cfg.AddConsumer<CurrierVisitedConsumer>();
            
            
            
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

                rider.AddProducer<IСurrierEntered>(nameof(IСurrierEntered));
                rider.AddProducer<IСurrierLeft>(nameof(IСurrierLeft));

                // duplicative, since it's already published to RabbitMQ, but showing how to also
                // produce an event on Kafka from a state machine
                rider.AddProducer<ICurrierVisited>(nameof(ICurrierVisited));

                rider.UsingKafka((context, k) =>
                {
                    k.Host("localhost:9092");

                    k.TopicEndpoint<Null, IСurrierEntered>(nameof(IСurrierEntered), nameof(Assembly.GetName), c =>
                    {
                        c.AutoOffsetReset = AutoOffsetReset.Earliest;
                        c.CreateIfMissing(t => t.NumPartitions = 1);
                        c.ConfigureSaga<СurrierState>(context);
                    });

                    k.TopicEndpoint<Null, IСurrierLeft>(nameof(IСurrierLeft), nameof(Assembly.GetName), c =>
                    {
                        c.AutoOffsetReset = AutoOffsetReset.Earliest;
                        c.CreateIfMissing(t => t.NumPartitions = 1);
                        c.ConfigureSaga<СurrierState>(context);
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
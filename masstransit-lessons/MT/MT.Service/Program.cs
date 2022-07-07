using System.Data;
using MassTransit;
using MassTransit.Courier.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using MT.SampleComponents.BatchConsumers;
using MT.SampleComponents.Consumers;
using MT.SampleComponents.CourierAcitivities;
using MT.SampleComponents.StateMachine;
using StackExchange.Redis;

await Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((hostingConext, config) => { config.AddJsonFile("appsettings.json", true); })
    .ConfigureServices((hostContext, services) =>
    {
        services.TryAddSingleton(KebabCaseEndpointNameFormatter
            .Instance); // позже напиши. kebab case лучше чем snake _ kase

        services.AddMassTransit(cfg =>
        {
            cfg.AddConsumersFromNamespaceContaining<SubmitOrderConsumer>();
            cfg.AddActivitiesFromNamespaceContaining<AllocateInventoryActivity>();


            cfg.AddConsumer<RoutingSlipBatchEventConsumer,RoutingSlipBatchEventConsumerDefinition>();
            cfg.AddConsumer<SubmitOrderConsumer, SubmitOrderDefinition>();
            cfg.AddConsumer<AnyFaultConsumer>();
            cfg.AddConsumer<SubmitOrderFaultConsumer>();
            cfg.AddSagaStateMachine<OrderStateMachine, OrderState>(typeof(OrderStateMachineDefinition))
                .MongoDbRepository(r =>
                {
                    r.Connection = "mongodb://127.0.0.1";
                    r.DatabaseName = "orderdb";
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
        logging.AddConsole(x =>
        {
            
        });
    })
    .Build()
    .RunAsync();
using System.Data;
using System.Reflection;
using Delivery.Components.Consumers;
using Delivery.Components.StateMachines;
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
            cfg.AddConsumer<DeliverOrderConsumer>();

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
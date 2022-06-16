// See https://aka.ms/new-console-template for more information

using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Configuration;
using Microsoft.Extensions.Options;
using Quartz;
using Quartz.Impl;
using Warehouse.Components.Consumers;
using Warehouse.Components.StateMachines;

await Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((hostingConext, config) => { config.AddJsonFile("appsettings.json", true); })
    .ConfigureServices((hostContext, services) =>
    {
        services.TryAddSingleton(KebabCaseEndpointNameFormatter
            .Instance); // позже напиши. kebab case лучше чем snake _ kase

        services.Configure<QuartzConfig>(hostContext.Configuration.GetSection("Quartz"));

        services.AddMassTransit(cfg =>
        {
            cfg.AddConsumersFromNamespaceContaining<AllocateInventoryConsumer>();
            cfg.AddSagaStateMachine<AllocationStateMachine, AllocationState>()
                .MongoDbRepository(r =>
                {
                    r.Connection = "mongodb://127.0.0.1";
                    r.DatabaseName = "allocation-db";
                });

            cfg.UsingRabbitMq((context, config) =>
            {
                config.Host("localhost", "work", h =>
                {
                    h.Username("guest");
                    h.Password("guest");
                });
                
                
                config.UseMessageScheduler(new Uri("queue:quartz"));
                
               /* Uri schedulerEndpoint = new Uri("queue:scheduler");
                config.UseMessageScheduler(schedulerEndpoint);
                */
               
               
               /* config.UseInMemoryScheduler(x =>
                {
                    x.SchedulerFactory = new StdSchedulerFactory(context.GetService<IOptions<QuartzConfig>>().Value
                        .ToNameValueCollection());
                });
*/
                config.ConfigureEndpoints(context);
            });
        });
    })
    .ConfigureLogging((hostingContext, logging) =>
    {
        logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
        logging.AddConsole();
    })
    .Build()
    .RunAsync();
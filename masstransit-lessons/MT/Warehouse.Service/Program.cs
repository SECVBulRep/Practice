// See https://aka.ms/new-console-template for more information

using System.Configuration;
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
using Quartz.Spi;
using Warehouse.Components.Consumers;
using Warehouse.Components.StateMachines;

await Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((hostingConext, config) => { config.AddJsonFile("appsettings.json", true); })
    .ConfigureServices((hostContext, services) =>
    {
        services.TryAddSingleton(KebabCaseEndpointNameFormatter
            .Instance); // позже напиши. kebab case лучше чем snake _ kase


        services.Configure<QuartzOptions>(hostContext.Configuration.GetSection("Quartz"));


        services.Configure<QuartzOptions>(options =>
        {
            options.Scheduling.IgnoreDuplicates = true;
            options.Scheduling.OverWriteExistingData = true;
        });

        services.AddQuartz(q =>
        {
            q.SchedulerId = "Scheduler-Core";
            q.UseMicrosoftDependencyInjectionJobFactory();
        });

        services.AddQuartzHostedService(options => { options.WaitForJobsToComplete = true; });

        const string schedulerQueueName = "scheduler";
        var schedulerUri = new Uri($"queue:{schedulerQueueName}");

        services.AddMassTransit(cfg =>
        {
            
            cfg.AddMessageScheduler(schedulerUri);

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


                config.UseInMemoryScheduler(schedulerCfg =>
                {
                    schedulerCfg.QueueName = schedulerQueueName;
                    schedulerCfg.SchedulerFactory = context.GetRequiredService<ISchedulerFactory>();
                    schedulerCfg.StartScheduler = false;
                });


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
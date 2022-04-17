// See https://aka.ms/new-console-template for more information

using System.Data;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MT.SampleComponents.Consumers;

await Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((hostingConext, config) => { config.AddJsonFile("appsettings.json", true); })
    .ConfigureServices((hostContext, services) =>
    {
        services.TryAddSingleton(KebabCaseEndpointNameFormatter
            .Instance); // позже напиши. kebab case лучше чем snake _ kase

        services.AddMassTransit(cfg =>
        {
            cfg.AddConsumersFromNamespaceContaining<SubmitOrderConsumer>();

            cfg.AddConsumer<SubmitOrderConsumer, SubmitOrderDefinition>();
            cfg.AddConsumer<AnyFaultConsumer>();
            cfg.AddConsumer<SubmitOrderFaultConsumer>();


            //cfg.AddBus(ConfigureBus); перепишем по другому 
            cfg.UsingRabbitMq((context, config) =>
            {
                config.Host("localhost", "work", h =>
                {
                    h.Username("guest");
                    h.Password("guest");
                });

                // для всех
                
                /*
                 None	No retry
                 Immediate	Retry immediately, up to the retry limit
                 Interval	Retry after a fixed delay, up to the retry limit
                 Intervals	Retry after a delay, for each interval specified
                 Exponential	Retry after an exponentially increasing delay, up to the retry limit
                 Incremental	Retry after a steadily increasing delay, up to the retry limit
                 * 
                 */

                //config.UseMessageRetry(r => r.Immediate(5));
                
                
                // для конкретного
                config.ReceiveEndpoint($"{KebabCaseEndpointNameFormatter.Instance.Consumer<SubmitOrderConsumer>()}", e =>
                {
                    /*фильтр для всего ендпойнта*/
                    e.UseMessageRetry(r =>
                    {
                        r.Immediate(5);
                        r.Handle<DataException>(x=>x.Message.Contains("SQL"));
                    });
                    
                   
                    /*фильтр для конкретного консюмера*/
                    /*e.ConfigureConsumer<SubmitOrderConsumer>(context,c=>c.UseMessageRetry(r =>
                    {
                        r.Interval(10, TimeSpan.FromMilliseconds(200));
                        r.Ignore<ArgumentException>();
                        r.Ignore<DataException>(x=>x.Message.Contains("SQL"));
                    }));*/
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

IBusControl ConfigureBus(IBusRegistrationContext serviceProvider)
{
    return Bus.Factory.CreateUsingRabbitMq(cfg =>
    {
        cfg.Host("localhost", "work", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });

        cfg.UseMessageRetry(r => r.Immediate(5));

        cfg.ConfigureEndpoints(serviceProvider);
    });
}
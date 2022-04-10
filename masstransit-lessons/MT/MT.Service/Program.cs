// See https://aka.ms/new-console-template for more information
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
            cfg.AddBus(ConfigureBus);
        });

        services.AddMassTransitHostedService(); // убрать абсолет
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
        cfg.Host("172.16.29.35", "masstransit-test", h =>
        {
            h.Username("bulat");
            h.Password("123qweASD");
        });
        cfg.ConfigureEndpoints(serviceProvider);
    });
}
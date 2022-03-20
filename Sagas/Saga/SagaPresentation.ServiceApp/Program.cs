// See https://aka.ms/new-console-template for more information

using MassTransit;
using Microsoft.Extensions.Hosting;

await Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddMassTransit(x =>
        {
            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host("172.16.29.35", "NPD", h =>
                {
                    h.Username("bulat");
                    h.Password("123qweASD");
                });

                cfg.ConfigureEndpoints(context);
            });
        });

        services.AddMassTransitHostedService();
    })
    .Build()
    .RunAsync();
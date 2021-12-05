using System;
using MassTransit;
using MassTransit.RabbitMqTransport;


namespace RB.Core
{
    public class RabbitMqBus
    {
        public static IBusControl ConfigureBus(IServiceProvider provider, Action<IRabbitMqBusFactoryConfigurator, IRabbitMqHost>
            registrationAction = null)
        {
            return Bus.Factory.CreateUsingRabbitMq(cfg =>
            {
                cfg.Host(new Uri(BusConstants.RabbitMqUri),BusConstants.Virt, hst =>
                {
                    hst.Username(BusConstants.UserName);
                    hst.Password(BusConstants.Password);
                });
                
                /* cfg.ConfigureEndpoints(provider);
                registrationAction?.Invoke(cfg, host);*/
            });
        }
    }
}
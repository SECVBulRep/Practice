using MassTransit;
using MassTransit.ExtensionsDependencyInjectionIntegration;
using MassTransit.ExtensionsDependencyInjectionIntegration.MultiBus;
using MassTransit.Platform.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Warehouse.Components.Consumers;
using Warehouse.Components.StateMachines;

namespace Warehouse.Startup;

public class WarehouseStartup: IPlatformStartup
{

    public void ConfigureMassTransit(IServiceCollectionBusConfigurator configurator, IServiceCollection services)
    {
        throw new NotImplementedException();
    }

    public void ConfigureBus<TEndpointConfigurator>(IBusFactoryConfigurator<TEndpointConfigurator> configurator, IBusRegistrationContext context) where TEndpointConfigurator : IReceiveEndpointConfigurator
    {
        throw new NotImplementedException();
    }
}
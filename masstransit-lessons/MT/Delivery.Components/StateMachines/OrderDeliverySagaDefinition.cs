using MassTransit;

namespace Delivery.Components.StateMachines;

public class OrderDeliverySagaDefinition :
    SagaDefinition<OrderDeliveryState>
{
    public OrderDeliverySagaDefinition()
    {
        Endpoint(x => x.ConcurrentMessageLimit = 1);
    }

    protected override void ConfigureSaga(IReceiveEndpointConfigurator endpointConfigurator, ISagaConfigurator<OrderDeliveryState> sagaConfigurator)
    {
        endpointConfigurator.UseMessageRetry(x=>x.Intervals(500,1000,5000,30000));
        endpointConfigurator.UseInMemoryOutbox();
    }
}
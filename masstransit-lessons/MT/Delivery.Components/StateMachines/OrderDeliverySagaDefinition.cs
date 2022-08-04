using Delivery.Contracts;
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
        endpointConfigurator.UseFilter(new CatchMeIfYouCan());
    }
}


public class CatchMeIfYouCan : IFilter<ConsumeContext>
{
    public async Task Send(ConsumeContext context, IPipe<ConsumeContext> next)
    {
        await next.Send(context).ConfigureAwait(false);
        Console.WriteLine("Hello");
    }
    public void Probe(ProbeContext context)
    {
    }
}
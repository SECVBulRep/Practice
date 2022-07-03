using MassTransit;

namespace MT.SampleComponents.Consumers;

public class FulfillOrderConsumerdefinition : ConsumerDefinition<FulfillOrderConsumer>
{
    protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<FulfillOrderConsumer> consumerConfigurator)
    {
        base.ConfigureConsumer(endpointConfigurator, consumerConfigurator);
        
        endpointConfigurator.UseMessageRetry(r=>
        {
            //потому что мы знаем что они никогда не примется и переотправлять его смысла нет
            r.Ignore<InvalidOperationException>();
            r.Interval(3, 1000);
        });
    }
}
using MassTransit;

namespace MT.SampleComponents.Consumers;

public class SubmitOrderDefinition : ConsumerDefinition<SubmitOrderConsumer>
{
    protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator,
        IConsumerConfigurator<SubmitOrderConsumer> consumerConfigurator)
    {
        base.ConfigureConsumer(endpointConfigurator, consumerConfigurator);
        endpointConfigurator.PublishFaults = true;
        endpointConfigurator.UseMessageRetry(r=>r.Interval(3,1000));
    }
}
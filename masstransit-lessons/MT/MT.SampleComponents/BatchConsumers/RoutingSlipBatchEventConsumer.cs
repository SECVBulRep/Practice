using MassTransit;
using MassTransit.Courier.Contracts;
using Microsoft.Extensions.Logging;
using MT.SampleComponents.Consumers;

namespace MT.SampleComponents.BatchConsumers;

public class RoutingSlipBatchEventConsumer :
    IConsumer<Batch<RoutingSlipCompleted>>
{
        
    private ILogger<RoutingSlipEventConsumer> _logger;

    public RoutingSlipBatchEventConsumer(ILogger<RoutingSlipEventConsumer> logger)
    {
        _logger = logger;
    }
    public Task Consume(ConsumeContext<Batch<RoutingSlipCompleted>> context)
    {
        _logger.LogInformation($"Routing slip completed. Tracking number" +
                               $" { string.Join(",", context.Message.Select(x=>x.Message.TrackingNumber))}");
        return Task.CompletedTask;
    }
}

public class RoutingSlipBatchEventConsumerDefinition :
    ConsumerDefinition<RoutingSlipBatchEventConsumer>
{
    public RoutingSlipBatchEventConsumerDefinition()
    {
    }

    protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator,
        IConsumerConfigurator<RoutingSlipBatchEventConsumer> consumerConfigurator)
    {
        consumerConfigurator.Options<BatchOptions>(options => options
            .SetMessageLimit(5)
            .SetTimeLimit(1000));
    }
}
using Delivery.Components.Filters;
using Delivery.Contracts;
using MassTransit;
using MassTransit.Middleware;
using Microsoft.Extensions.Logging;

namespace Delivery.Components.Consumers;

public class DeliverOrderConsumer :
    IConsumer<IDeliverOrder>
{
    readonly ILogger<DeliverOrderConsumer> _logger;

    public DeliverOrderConsumer(ILogger<DeliverOrderConsumer> logger)
    {
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<IDeliverOrder> context)
    {
        _logger.LogInformation($"Delivering order: {context.Message.OrderId}");
        await Task.Delay(1000);
    }
}


public class DeliverOrderConsumerDefitnition : ConsumerDefinition<DeliverOrderConsumer>
{
    protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<DeliverOrderConsumer> consumerConfigurator)
    {
        endpointConfigurator.UseFilter(new ConsoleConsumeFilter());
        consumerConfigurator.UseFilter(new ConsoleConsumeWithConsumerFilter<DeliverOrderConsumer>());
        consumerConfigurator.ConsumerMessage<IDeliverOrder>(m=>m.UseFilter(new ConsoleConsumeWithConsumerFilter<DeliverOrderConsumer, IDeliverOrder>()));
    }
} 
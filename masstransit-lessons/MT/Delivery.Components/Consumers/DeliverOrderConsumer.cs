using Delivery.Contracts;
using MassTransit;
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
using Delivery.Contracts;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Delivery.Components.Consumers;

public class CurrierVisitedConsumer :
    IConsumer<ICurrierVisited>
{
    readonly ILogger<CurrierVisitedConsumer> _logger;

    public CurrierVisitedConsumer(ILogger<CurrierVisitedConsumer> logger)
    {
        _logger = logger;
    }

    public Task Consume(ConsumeContext<ICurrierVisited> context)
    {
        _logger.LogInformation("Сurrier Visited: СurrierId {СurrierId} Entered {Entered} Left {Left} Duration {Duration}", context.Message.СurrierId,
            context.Message.Entered, context.Message.Left, context.Message.Left - context.Message.Entered);

        return Task.CompletedTask;
    }
}
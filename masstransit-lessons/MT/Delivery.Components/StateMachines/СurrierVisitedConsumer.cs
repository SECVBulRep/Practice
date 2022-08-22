using Delivery.Contracts;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Delivery.Components.StateMachines;

public class СurrierVisitedConsumer :
    IConsumer<IСurrierVisited>
{
    readonly ILogger<СurrierVisitedConsumer> _logger;

    public СurrierVisitedConsumer(ILogger<СurrierVisitedConsumer> logger)
    {
        _logger = logger;
    }

    public Task Consume(ConsumeContext<IСurrierVisited> context)
    {
        _logger.LogInformation("Сurrier Visited: {СurrierId} {Entered} {Left} {Duration}", context.Message.СurrierId,
            context.Message.Entered, context.Message.Left, context.Message.Left - context.Message.Entered);

        return Task.CompletedTask;
    }
}
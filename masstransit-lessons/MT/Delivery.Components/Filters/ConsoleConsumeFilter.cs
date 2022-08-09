using MassTransit;

namespace Delivery.Components.Filters;

public class ConsoleConsumeFilter : IFilter<ConsumeContext>
{
    public async  Task Send(ConsumeContext context, IPipe<ConsumeContext> next)
    {
        Console.WriteLine($"Consume: {context.MessageId}");
        await next.Send(context);
    }

    public void Probe(ProbeContext context)
    {
        context.CreateScope("ConsoleConsumeFilter");
        context.Add("output", "console");
    }
}
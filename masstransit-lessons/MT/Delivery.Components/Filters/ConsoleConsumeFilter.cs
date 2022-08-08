using MassTransit;

namespace Delivery.Components.Filters;

public class ConsoleConsumeFilter : IFilter<ConsumeContext>
{
    public async Task Send(ConsumeContext context, IPipe<ConsumeContext> next)
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


public class ConsoleConsumeWithConsumerFilter<TConsumer> : IFilter<ConsumerConsumeContext<TConsumer>> where TConsumer : class
{
    public async Task Send(ConsumerConsumeContext<TConsumer> context, IPipe<ConsumerConsumeContext<TConsumer>> next)
    {
        Console.WriteLine($"Consume With Consumer : {context.MessageId}");
        await next.Send(context);
    }

    public void Probe(ProbeContext context)
    {
        context.CreateScope("ConsoleWithConsumerConsumeFilter");
        context.Add("output", "console");
    }
}
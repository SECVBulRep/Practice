﻿using MassTransit;

namespace Delivery.Components.Filters;

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


public class ConsoleConsumeWithConsumerFilter<TConsumer,TMessage> : IFilter<ConsumerConsumeContext<TConsumer, TMessage>> where TConsumer : class where TMessage : class
{
    public async Task Send(ConsumerConsumeContext<TConsumer, TMessage> context, IPipe<ConsumerConsumeContext<TConsumer, TMessage>> next)
    {
        Console.WriteLine($"Consume With Consumer : {context.MessageId} message type {typeof(TMessage).Name}");
        await next.Send(context);
    }

    public void Probe(ProbeContext context)
    {
        context.CreateScope("ConsoleWithConsumerConsumeFilter");
        context.Add("output", "console");
    }
}
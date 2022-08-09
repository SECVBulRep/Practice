using MassTransit;
using MassTransit.Configuration;

namespace Delivery.Components.Filters;


public class ConsoleConsumerMessageFilter<TMessage> : IFilter<ConsumeContext<TMessage>> 
    where TMessage : class
{
    public async Task Send(ConsumeContext<TMessage> context, IPipe<ConsumeContext<TMessage>> next)
    {
        Console.WriteLine($"!!! FilterConfigurationObserver !!! Consume With Consumer : {context.MessageId} message type {typeof(TMessage).Name}");
        await next.Send(context);
    }

    public void Probe(ProbeContext context)
    {
        context.CreateScope("ConsoleConsumeFilter");
        context.Add("output", "console");
    }
}


public class ConsoleConsumerMessageFilterConfigurationObserver : IConsumerConfigurationObserver
{
    private readonly IConsumePipeConfigurator _configuration;

    public ConsoleConsumerMessageFilterConfigurationObserver(IConsumePipeConfigurator configuration)
    {
        _configuration = configuration;
    }
    public void ConsumerConfigured<TConsumer>(IConsumerConfigurator<TConsumer> configurator) where TConsumer : class
    {
       
    }

    public void ConsumerMessageConfigured<TConsumer, TMessage>(
        IConsumerMessageConfigurator<TConsumer, TMessage> configurator) where TConsumer : class
        where TMessage : class
    {
        _configuration.AddPipeSpecification(new FilterPipeSpecification<ConsumeContext<TMessage>>(new ConsoleConsumerMessageFilter<TMessage>()));
    }
}
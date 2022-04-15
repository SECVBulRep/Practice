using MassTransit;

namespace MT.SampleComponents.Consumers;

public class AnyFaultConsumer : IConsumer<Fault>
{
    public Task Consume(ConsumeContext<Fault> context)
    {
        return Task.CompletedTask;
    }
}
using MassTransit;
using MT.SampleContracts;

namespace MT.SampleComponents.Consumers;

public class SubmitOrderFaultConsumer : IConsumer<Fault<ISubmitOrder>>
{
    public Task Consume(ConsumeContext<Fault<ISubmitOrder>> context)
    {
        return Task.CompletedTask;
    }
}
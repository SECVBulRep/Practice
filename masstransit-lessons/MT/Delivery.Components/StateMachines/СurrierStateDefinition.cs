using Delivery.Contracts;
using MassTransit;
using MassTransit.Middleware;

namespace Delivery.Components.StateMachines;

public class СurrierStateDefinition :
    SagaDefinition<СurrierState>
{
    readonly IPartitioner _partition;

    public СurrierStateDefinition()
    {
        _partition = new Partitioner(64, new Murmur3UnsafeHashGenerator());
    }

    protected override void ConfigureSaga(IReceiveEndpointConfigurator endpointConfigurator, ISagaConfigurator<СurrierState> sagaConfigurator)
    {
        sagaConfigurator.Message<IСurrierEntered>(x => x.UsePartitioner(_partition, m => m.Message.СurrierId));
        sagaConfigurator.Message<IСurrierLeft>(x => x.UsePartitioner(_partition, m => m.Message.СurrierId));

        endpointConfigurator.UseMessageRetry(r => r.Intervals(20, 50, 100, 1000, 5000));
    }
}
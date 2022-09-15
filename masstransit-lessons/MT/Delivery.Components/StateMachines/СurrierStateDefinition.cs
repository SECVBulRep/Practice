using confluent.io.examples.serialization.avro;
using Confluent.Kafka.Examples.AvroSpecific;
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
        sagaConfigurator.Message<ICurrierEntered>(x => x.UsePartitioner(_partition, m => m.Message.CurrierId));
        sagaConfigurator.Message<ICurrierLeft>(x => x.UsePartitioner(_partition, m => m.Message.CurrierId));
        endpointConfigurator.UseMessageRetry(r => r.Intervals(20, 50, 100, 1000, 5000));
    }
}
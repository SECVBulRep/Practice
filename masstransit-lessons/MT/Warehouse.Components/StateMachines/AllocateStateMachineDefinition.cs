using MassTransit;

namespace Warehouse.Components.StateMachines;

public class AllocateStateMachineDefinition : SagaDefinition<AllocationState>
{
    public AllocateStateMachineDefinition()
    {
        ConcurrentMessageLimit = 4;
    }

    protected override void ConfigureSaga(IReceiveEndpointConfigurator endpointConfigurator, ISagaConfigurator<AllocationState> sagaConfigurator)
    {
        base.ConfigureSaga(endpointConfigurator, sagaConfigurator);
        endpointConfigurator.UseMessageRetry(r=>r.Interval(3,1000));
        endpointConfigurator.UseInMemoryOutbox();
    }
}
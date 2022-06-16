using MassTransit;

namespace Warehouse.Components.StateMachines;

public class AllocationState : SagaStateMachineInstance, ISagaVersion
{
    public Guid CorrelationId { get; set; }
    public int Version { get; set; }

    /// <summary>
    /// токен для отмены запланированного задания
    /// </summary>
    public Guid? HoldDurationToken { get; set; }

    public string CurrentState { get; set; }
}
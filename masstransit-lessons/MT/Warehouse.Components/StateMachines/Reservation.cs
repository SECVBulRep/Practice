using MassTransit;

namespace Warehouse.Components.StateMachines;

public class Reservation : SagaStateMachineInstance
{
    public Guid CorrelationId { get; set; }
    public int CurrentState { get; set; }

    public DateTime Created { get; set; }

    public Guid ClientId { get; set; }
    
    public Guid ProductId { get; set; }
}
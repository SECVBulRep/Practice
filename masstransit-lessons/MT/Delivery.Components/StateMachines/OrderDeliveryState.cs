using MassTransit;

namespace Delivery.Components.StateMachines;

public class OrderDeliveryState :
    SagaStateMachineInstance
{
    public int CurrentState { get; set; }
    public Guid OrderId { get; set; }
    public DateTime RequestTimestamp { get; set; }
    public Guid CorrelationId { get; set; }
}
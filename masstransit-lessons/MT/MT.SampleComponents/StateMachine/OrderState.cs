using MassTransit;

namespace MT.SampleComponents.StateMachine;

public class OrderState : SagaStateMachineInstance, ISagaVersion
{
    public Guid CorrelationId { get; set; }
    public int Version { get; set; }
    public string CurrentState { get; set; }
    public string CustomerNumber { get; set; }
    public DateTime? SubmitDate { get; set; }
    public DateTime? Updated { get; set; }
    public string FaultReason { get; set; }
    public string PaymentCardNumber { get; set; }
}
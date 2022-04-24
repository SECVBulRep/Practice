using System.Net.Mail;
using MassTransit;
using MT.SampleContracts;

namespace MT.SampleComponents.StateMachine;

public class OrerStateMachine : MassTransitStateMachine<OrderState>
{
    public OrerStateMachine()
    {
        Event(() => OrderSubmitted, x => x.CorrelateById(m => m.Message.OrderId));

        InstanceState(x => x.CurrentState);

        Initially(
            When(OrderSubmitted)
                .TransitionTo(Submitted)
        );
    }

    public State Submitted { get; set; }
    public Event<IOrderSubmitted> OrderSubmitted { set; get; }
}

public class OrderState : SagaStateMachineInstance, ISagaVersion
{
    public Guid CorrelationId { get; set; }
    public string CurrentState { get; set; }
    public int Version { get; set; }
}
using Delivery.Contracts;
using MassTransit;

namespace Delivery.Components.StateMachines;

public class OrderDeliveryStateMachine :
    MassTransitStateMachine<OrderDeliveryState>
{
    public OrderDeliveryStateMachine()
    {
        InstanceState(instance => instance.CurrentState, Requested);

        Event(() => OrderDeliveryRequested,
            x => { x.CorrelateById(instance => instance.OrderId, context => context.Message.OrderId); });

        Initially(
            When(OrderDeliveryRequested)
                .Then(x =>
                {
                    x.Saga.OrderId = x.Message.OrderId;
                    x.Saga.RequestTimestamp = x.GetPayload<ConsumeContext>().SentTime ?? DateTime.UtcNow;
                })
                .TransitionTo(Requested));
    }

    public State Requested { get; }

    public Event<IOrderDeliveryRequesting> OrderDeliveryRequested { get; }
}
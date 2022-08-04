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
            x => { x.CorrelateById(context => context.Message.OrderId); });

        Initially(
            When(OrderDeliveryRequested)
                .Then(x =>
                {
                    x.Saga.OrderId = x.Message.OrderId;
                    x.Saga.RequestTimestamp = x.GetPayload<ConsumeContext>().SentTime ?? DateTime.UtcNow;
                })
                .PublishAsync(x=>x.Init<IDeliverOrder>(new
                {
                    x.Message.OrderId
                }))
                .TransitionTo(Requested));
    }

    public State Requested { get; }

    public Event<IOrderDeliveryRequesting> OrderDeliveryRequested { get; }
}
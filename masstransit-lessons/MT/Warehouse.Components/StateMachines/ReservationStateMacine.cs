using MassTransit;
using Warehouse.Contracts;

namespace Warehouse.Components.StateMachines;

public class ReservationStateMacine : MassTransitStateMachine<Reservation>
{
    static ReservationStateMacine()
    {
        MessageContracts.Initialize();
    }

    public ReservationStateMacine()
    {
        InstanceState(x => x.CurrentState, Requested, Reserved);

        Initially(
            When(ReservationRequested)
                .Then(context =>
                {
                    context.Saga.Created = context.Message.TimeStamp;
                    context.Saga.ClientId = context.Message.ClientId;
                    context.Saga.ProductId = context.Message.ProductId;
                })
                .TransitionTo(Requested)
            );
    }

    public State Requested { get; }
    public State Reserved { get; }

    public Event<IReservationRequested> ReservationRequested { get; set; }
}
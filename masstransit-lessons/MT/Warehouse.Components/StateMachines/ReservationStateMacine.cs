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
        Event(() => ProductReserved, x => x.CorrelateById(m => m.Message.ReservationId));
        
        
        
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
        
        
        During(Requested,
            When(ProductReserved)
                .Then(x =>
                {
                    x.Saga.Reserved = x.Message.TimeStamp;
                })
                .TransitionTo(Reserved)
            );
        
    }

   

    public State Requested { get; }
    public State Reserved { get; }

    public Event<IReservationRequested> ReservationRequested { get; set; }
    public Event<IProductReserved> ProductReserved { get; set; }
}
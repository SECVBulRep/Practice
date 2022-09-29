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
        
        Schedule(() => ExpiationSchedule, x => x.ExpirationTokenId, x => x.Delay = TimeSpan.FromHours(24));

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
                .Schedule(ExpiationSchedule,context=>context.Init<IReservationExpired>(new {context.Message.ReservationId}))
                .TransitionTo(Reserved)
            );
        
    }

   

    public State Requested { get; }
    public State Reserved { get; }

    public Schedule<Reservation, IReservationExpired> ExpiationSchedule { get; set; }
    public Event<IReservationRequested> ReservationRequested { get; set; }
    public Event<IProductReserved> ProductReserved { get; set; }
    public Event<IReservationExpired> ReservationExpired { get; set; }
}
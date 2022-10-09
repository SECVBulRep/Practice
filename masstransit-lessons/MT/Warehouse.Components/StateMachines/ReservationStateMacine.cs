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
        Event(() => ProductCheckedOut,
            x => x.CorrelateBy((instance, context) => instance.ProductId == context.Message.ProductId));


        Schedule(() => ExpiationSchedule, x => x.ExpirationTokenId, x => x.Delay = TimeSpan.FromHours(24));

        Initially(
            When(ReservationRequested)
                .Then(context =>
                {
                    context.Saga.Created = context.Message.TimeStamp;
                    context.Saga.ClientId = context.Message.ClientId;
                    context.Saga.ProductId = context.Message.ProductId;
                })
                .TransitionTo(Requested),
            When(ProductReserved)
                .Then(context =>
                {
                    context.Saga.Created = context.Message.TimeStamp;
                    context.Saga.ClientId = context.Message.ClientId;
                    context.Saga.ProductId = context.Message.ProductId;
                    context.Saga.Reserved = context.Message.TimeStamp;
                })
                .Schedule(ExpiationSchedule,
                    context => context.Init<IReservationExpired>(new {context.Message.ReservationId}),
                    context => context.Message.Duration ?? TimeSpan.FromDays(1))
                .TransitionTo(Reserved),
            When(ReservationExpired)
                .Finalize()
        );


        During(Requested,
            Ignore(ReservationRequested),
            When(ProductReserved)
                .Then(x => { x.Saga.Reserved = x.Message.TimeStamp; })
                .Schedule(ExpiationSchedule,
                    context => context.Init<IReservationExpired>(new {context.Message.ReservationId}),
                    context => context.Message.Duration ?? TimeSpan.FromDays(1))
                .TransitionTo(Reserved)
        );


        During(Reserved,
            Ignore(ReservationRequested),
            When(ProductReserved) // можем запустить еще раз. ничего негативного не будет
                .Schedule(ExpiationSchedule,
                    context => context.Init<IReservationExpired>(new {context.Message.ReservationId}),
                    context => context.Message.Duration ?? TimeSpan.FromDays(1))
            , When(ReservationExpired)
                .PublishReservationCanceled()
                .Finalize()
            , When(ReservationCancelationRequested)
                .PublishReservationCanceled()
                .Unschedule(ExpiationSchedule)
                .Finalize()
            , When(ProductCheckedOut)
                .Unschedule(ExpiationSchedule)
                .Finalize()
        );


        SetCompletedWhenFinalized();
    }


    public State Requested { get; }
    public State Reserved { get; }

    public Event<IProductCheckedOut> ProductCheckedOut { get; }
    public Schedule<Reservation, IReservationExpired> ExpiationSchedule { get; set; }
    public Event<IReservationRequested> ReservationRequested { get; set; }

    public Event<IReservationCancelationRequested> ReservationCancelationRequested { get; set; }
    public Event<IProductReserved> ProductReserved { get; set; }
    public Event<IReservationExpired> ReservationExpired { get; set; }
}

public static class ReservationStateMachineExtensions
{
    public static EventActivityBinder<Reservation, T> PublishReservationCanceled<T>(
        this EventActivityBinder<Reservation, T> binder)
        where T : class
    {
        return binder.PublishAsync(context => context.Init<IProductReservationCanceled>(new
        {
            ProductId = context.Saga.ProductId
        }));
    }
}
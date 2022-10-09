using MassTransit;
using Warehouse.Contracts;

namespace Warehouse.Components.StateMachines;

public class ProductStateMachine :
    MassTransitStateMachine<Product>
{

    static ProductStateMachine()
    {
        MessageContracts.Initialize();
    }

    public ProductStateMachine()
    {
        InstanceState(x => x.CurrentState, Available,Reserved,CheckedOut);

        Event(() => ReservationRequested, x => x.CorrelateById(m => m.Message.ProductId));
        
        Initially(
            When(Added)
                .CopyDataToInstance()
                .TransitionTo(Available));
        
        
        During(Available,
            When(ReservationRequested)
                .Then(context=>context.Saga.ReservationId = context.Message.ReservationId)//  ниже не пишу потому что  ReservationId уже будет в этом случае.
                .PublishProductReserved()
                .TransitionTo(Reserved)
        );
        
        During(Reserved,
            When(ReservationRequested)
                .IfElse(
                    context => context.Saga.ReservationId.HasValue &&
                               context.Saga.ReservationId.Value == context.Message.ReservationId,
                    ifTrue => ifTrue.PublishProductReserved(), ifElse => ifElse));

        During(Reserved,
            When(ProductReservationCanceled)
                .TransitionTo(Available));
        
        
        During(Available,Reserved,
            When(ProductCheckedOut)
                //.Then(context=>context.Saga.ReservationId = default)
                .TransitionTo(CheckedOut)
        );
    }

    public Event<IProductCheckedOut> ProductCheckedOut { get; }
    public Event<IProductAdded> Added { get; }
    public Event<IReservationRequested> ReservationRequested { get; }
    
    public Event<IProductReservationCanceled> ProductReservationCanceled { get; }
    public State Available { get; }
    public State Reserved { get; set; }

    public State CheckedOut { get; }
}

public static class ProductStateMachineExtensions
{
    public static EventActivityBinder<Product, IProductAdded> CopyDataToInstance(
        this EventActivityBinder<Product, IProductAdded> binder)
    {
        return binder.Then(x =>
        {
            x.Instance.DateAdded = x.Data.Timestamp.Date;
            x.Instance.Name = x.Data.Name;
            x.Instance.ManufacturerId = x.Data.ManufacturerId;
        });
    }
    
    public static EventActivityBinder<Product, IReservationRequested> PublishProductReserved(
        this EventActivityBinder<Product, IReservationRequested> binder)
    {
        return binder.PublishAsync(context => context.Init<IProductReserved>(new
        {
            context.Message.ClientId,
            context.Message.ReservationId,
            context.Message.ProductId,
            TimeStamp= DateTime.Now,
            context.Message.Duration
        }));
    }
}


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
        InstanceState(x => x.CurrentState, Available,Reserved);

        Event(() => ReservationRequested, x => x.CorrelateById(m => m.Message.ProductId));
        
        Initially(
            When(Added)
                .CopyDataToInstance()
                .TransitionTo(Available));
        
        
        During(Available,
            When(ReservationRequested)
                .TransitionTo(Reserved)
                .PublishAsync(context=>context.Init<IProductReserved>(new
                {
                    context.Message.ClientId,
                    context.Message.ReservationId,
                    context.Message.ProductId,
                    TimeStamp= DateTime.Now
                }))
        
        );
    }

   

    public Event<IProductAdded> Added { get; }
    public Event<IReservationRequested> ReservationRequested { get; }
    public State Available { get; }
    public State Reserved { get; set; }
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
}
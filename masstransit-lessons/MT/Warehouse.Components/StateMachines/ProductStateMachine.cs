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
        InstanceState(x => x.CurrentState, Available);

        Initially(
            When(Added)
                .CopyDataToInstance()
                .TransitionTo(Available));
    }

    public Event<IProductAdded> Added { get; }

    public State Available { get; }
}

public static class BookStateMachineExtensions
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
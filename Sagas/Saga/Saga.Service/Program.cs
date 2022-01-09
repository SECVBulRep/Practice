using Automatonymous;
using MassTransit;
using MassTransit.Saga;
using Saga.Service;

var machine = new OrderStateMachine();
var repository = new InMemorySagaRepository<OrderState>();

var busControl = Bus.Factory.CreateUsingInMemory(cfg =>
{
    cfg.ReceiveEndpoint("order", e => { e.StateMachineSaga(machine, repository); });
});

var source = new CancellationTokenSource(TimeSpan.FromSeconds(10));
await busControl.StartAsync(source.Token);

try
{
    do
    {
        string? value = await Task.Run(() =>
        {
            Console.WriteLine("Enter message (or quit to exit)");
            Console.Write("> ");
            return Console.ReadLine();
        });
        if ("quit".Equals(value, StringComparison.OrdinalIgnoreCase))
            break;

        if ("1".Equals(value, StringComparison.OrdinalIgnoreCase))
        {
            
        }



    } while (true);
}
finally
{
    await busControl.StopAsync();
}

Console.WriteLine("app stoped");


namespace Saga.Service
{
    public class OrderState :
        SagaStateMachineInstance
    {
        public Guid CorrelationId { get; set; }
        public int CurrentState { get; set; }
        public DateTime? OrderDate { get; set; }
        public string OrderNumber { get; set; }
        public int ReadyEventStatus { get; set; }
    }

    public interface OrderCanceled :
        CorrelatedBy<Guid>
    {    
    }
    
    public interface SubmitOrder
    {
        Guid OrderId { get; }
        DateTime OrderDate { get; }
    }

    public interface OrderAccepted
    {
        Guid OrderId { get; }
    }
    
    public interface ExternalOrderSubmitted
    {    
        string OrderNumber { get; }
    }

    public interface OrderCompleted
    {    
        Guid OrderId { get; }
    }
    
    public class OrderStateMachine :
        MassTransitStateMachine<OrderState>
    {
        public State Submitted { get; private set; }
        public State Accepted { get; private set; }
        
        public State Canceled { get; private set; }
        public Event<SubmitOrder> SubmitOrder { get; private set; }
        public Event<OrderAccepted> OrderAccepted { get; private set; }
        public Event<OrderCanceled> OrderCanceled { get;  private set; }
        public Event<ExternalOrderSubmitted> ExternalOrderSubmitted { get; private set; }
        
        public Event<RequestOrderCancellation> OrderCancellationRequested { get; private set; }
        
        public Event<OrderCompleted> OrderCompleted { get; private set; }
        
        public Event OrderReady { get; private set; }
        
        public interface OrderSubmitted
        {
            Guid OrderId { get; }    
        }

        public interface RequestOrderCancellation
        {    
            Guid OrderId { get; }
        }

        public interface OrderNotFound
        {
            Guid OrderId { get; }
        }
        
        
        public OrderStateMachine()
        {
            InstanceState(x => x.CurrentState, Submitted, Accepted);

            
            Event(() => SubmitOrder, e => 
            {
                e.CorrelateById(context => context.Message.OrderId);

                e.InsertOnInitial = true;
                e.SetSagaFactory(context => new OrderState
                {
                    CorrelationId = context.Message.OrderId
                });
            });

            Event(() => OrderAccepted, x => x.CorrelateById(context => context.Message.OrderId));
            Event(() => OrderCanceled); // not required, as it is the default convention
            
            Event(() => ExternalOrderSubmitted, e =>
            {
                e.CorrelateBy(i => i.OrderNumber, x => x.Message.OrderNumber);
                e.SelectId(x => NewId.NextGuid());

                e.InsertOnInitial = true;
                e.SetSagaFactory(context => new OrderState
                {
                    CorrelationId = context.CorrelationId ?? NewId.NextGuid(),
                    OrderNumber = context.Message.OrderNumber,
                });
            });
            
            Event(() => OrderCompleted, x => x.CorrelateById(context => context.Message.OrderId));

            Event(() => OrderCancellationRequested, e =>
            {
                e.CorrelateById(context => context.Message.OrderId);

                e.OnMissingInstance(m =>
                {
                    return m.ExecuteAsync(x => x.RespondAsync<OrderNotFound>(new { x.Message.OrderId }));
                });
            });
            
          
            Initially(
                When(SubmitOrder)
                    .PublishAsync(context => context.Init<OrderSubmitted>(new { OrderId = context.Instance.CorrelationId }))
                    .TransitionTo(Submitted));
            
            DuringAny(
                When(OrderCancellationRequested)
                    .RespondAsync(context => context.Init<OrderCanceled>(new { OrderId = context.Instance.CorrelationId }))
                    .TransitionTo(Canceled));

            During(Submitted,
                When(OrderAccepted)
                    .TransitionTo(Accepted));

            During(Accepted,
                When(SubmitOrder)
                    .Then(x => x.Instance.OrderDate = x.Data.OrderDate));
            
            CompositeEvent(() => OrderReady, x => x.ReadyEventStatus, SubmitOrder, OrderAccepted);

            DuringAny(
                When(OrderReady)
                    .Then(context => Console.WriteLine("Order Ready: {0}", context.Instance.CorrelationId)));
            
            DuringAny(
                When(OrderCompleted)
                    .Finalize());
            SetCompletedWhenFinalized();
        }

        
    }
}
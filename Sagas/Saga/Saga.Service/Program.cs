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

    public class OrderStateMachine :
        MassTransitStateMachine<OrderState>
    {
        public State Submitted { get; private set; }
        public State Accepted { get; private set; }
        public Event<SubmitOrder> SubmitOrder { get; private set; }
        public Event<OrderAccepted> OrderAccepted { get; private set; }

        public OrderStateMachine()
        {
            InstanceState(x => x.CurrentState, Submitted, Accepted);

            Event(() => SubmitOrder, x => x.CorrelateById(context => context.Message.OrderId));
            Event(() => OrderAccepted, x => x.CorrelateById(context => context.Message.OrderId));

            Initially(
                When(SubmitOrder)
                    .Then(x => x.Instance.OrderDate = x.Data.OrderDate)
                    .TransitionTo(Submitted),
                When(OrderAccepted)
                    .TransitionTo(Accepted));

            During(Submitted,
                When(OrderAccepted)
                    .TransitionTo(Accepted));

            During(Accepted,
                When(SubmitOrder)
                    .Then(x => x.Instance.OrderDate = x.Data.OrderDate));
        }
    }
}
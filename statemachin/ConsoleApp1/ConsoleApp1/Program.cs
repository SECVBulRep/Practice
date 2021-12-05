using System;
using Automatonymous;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");


            Console.ReadKey();
        }
    }

    public interface SubmitOrder
    {
        Guid OrderId { get; }
    }

    public interface OrderAccepted
    {
        Guid OrderId { get; }    
    }
    
    public class OrderState :
        SagaStateMachineInstance
    {
        public Guid CorrelationId { get; set; }
        public int CurrentState { get; set; }
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
            
            Initially(
                When(SubmitOrder)
                    .TransitionTo(Submitted));
        }
    }
}
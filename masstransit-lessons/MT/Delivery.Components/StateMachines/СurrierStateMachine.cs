using confluent.io.examples.serialization.avro;
using Confluent.Kafka.Examples.AvroSpecific;
using Delivery.Contracts;
using MassTransit;

namespace Delivery.Components.StateMachines;

public sealed class СurrierStateMachine :
    MassTransitStateMachine<СurrierState>
{
    public СurrierStateMachine()
    {
        Event(() => Entered, x => x.CorrelateById(m => Guid.Parse(m.Message.CurrierId)));
        Event(() => Left, x => x.CorrelateById(m => Guid.Parse(m.Message.CurrierId)));

        // state интовый, поэтому надо перечислить все. 0 - None, 1 - Initial, 2 - Final, 3 - Tracking 
        InstanceState(x => x.CurrentState, Tracking);

        Initially(
            When(Entered)
                .Then(x =>
                {
                    Console.WriteLine($"Entered {x.Message.CurrierId} {x.Message.Timestamp}");
                })
                .Then(context => context.Saga.Entered = Convert.ToDateTime(context.Message.Timestamp))
                .TransitionTo(Tracking),
            When(Left)
                .Then(x =>
                {
                    Console.WriteLine($"Left {x.Message.CurrierId} {x.Message.Timestamp}");
                })
                .Then(context => context.Saga.Left = Convert.ToDateTime(context.Message.Timestamp))
                .TransitionTo(Tracking)
        );

        During(Tracking,
            When(Entered)
                .Then(x =>
                {
                    Console.WriteLine($"Entered {x.Message.CurrierId} {x.Message.Timestamp}");
                })
                .Then(context => context.Saga.Entered = Convert.ToDateTime(context.Message.Timestamp)),
            When(Left)
                .Then(x =>
                {
                    Console.WriteLine($"Left {x.Message.CurrierId} {x.Message.Timestamp}");
                })
                .Then(context => context.Saga.Left = Convert.ToDateTime(context.Message.Timestamp))
        );

        //  x.VisitedStatus  битовая маска для событии
        CompositeEvent(() => Visited, x => x.VisitedStatus, CompositeEventOptions.IncludeInitial, Entered, Left);

        DuringAny(
            When(Visited)
                .Then(context => Console.WriteLine("Visited: {0}", context.Saga.CorrelationId))

                // Publish will go to RabbitMQ, via the bus
                .PublishAsync(context => context.Init<ICurrierVisited>(new
                {
                    СurrierId = context.Saga.CorrelationId,
                    context.Saga.Entered,
                    context.Saga.Left
                }))

                // Produce will go to Kafka
                .Produce(context => context.Init<ICurrierVisited>(new
                {
                    СurrierId = context.Saga.CorrelationId,
                    context.Saga.Entered,
                    context.Saga.Left
                }))
                .Finalize()
        );

        SetCompletedWhenFinalized();
    }

    public State Tracking { get; private set; }
    public Event<ICurrierEntered> Entered { get; private set; }
    public Event<ICurrierLeft> Left { get; private set; }
    public Event Visited { get; private set; }
}
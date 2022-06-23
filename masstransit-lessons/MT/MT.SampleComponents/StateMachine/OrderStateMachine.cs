using System.Diagnostics;
using System.Net.Mail;
using MassTransit;
using MT.SampleComponents.StateMachine.OrderStateMachineActivities;
using MT.SampleContracts;

namespace MT.SampleComponents.StateMachine;

public class OrderStateMachine : MassTransitStateMachine<OrderState>
{
    public OrderStateMachine()
    {
        Event(() => OrderSubmitted, x => x.CorrelateById(m => m.Message.OrderId));
        Event(() => FulfillmentFaulted, x => x.CorrelateById(m => m.Message.OrderId));
        Event(() => FulfillmentCompleted, x => x.CorrelateById(m => m.Message.OrderId));

        Event(() => OrderStatusRequested, x =>
            {
                x.CorrelateById(m => m.Message.OrderId);
                x.OnMissingInstance(m => m.ExecuteAsync(async context =>
                {
                    if (context.RequestId.HasValue)
                    {
                        await context.RespondAsync<IOrderNotFound>(new
                        {
                            context.Message.OrderId
                        });
                    }
                }));
            }
        );

        Event(() => AccountClosed,
            x => x.CorrelateBy(
                (saga, context) => saga.CustomerNumber == context.Message.CustomerNumber
            )
        );

        Event(() => OrderAccepted, x => x.CorrelateById(m => m.Message.OrderId));

        InstanceState(x => x.CurrentState);

        Initially(
            When(OrderSubmitted)
                .Then(context =>
                    {
                        context.Saga.SubmitDate = context.Message.TimeStamp;
                        context.Saga.CustomerNumber = context.Message.CustomerNumber;
                        context.Saga.Updated = DateTime.UtcNow;
                        context.Saga.PaymentCardNumber = context.Message.PaymentCardNumber;
                    }
                )
                .TransitionTo(Submitted)
        );

        //для идемпотенности
        During(Submitted,
            Ignore(OrderSubmitted),
            When(AccountClosed)
                .TransitionTo(Canceled),
            When(OrderAccepted)
                .Activity(x => x.OfType<OrderAcceptActivity>()
                    .TransitionTo(Accepted))
        );

        During(Accepted,
            When(FulfillmentFaulted)
                .TransitionTo(Faulted),
            When(FulfillmentCompleted)
                .TransitionTo(Completed)
        );


        // если мы хотим как то дополнить даные потом 
        DuringAny(
            When(OrderSubmitted)
                .Then(context =>
                {
                    context.Saga.SubmitDate ??= context.Message.TimeStamp;
                    context.Saga.CustomerNumber ??= context.Message.CustomerNumber;
                })
        );

        // возврат состяония саги
        DuringAny(
            When(OrderStatusRequested)
                .RespondAsync(x =>
                {
                    var res = x.Init<IOrderStatus>(new
                    {
                        OrderId = x.Saga.CorrelationId,
                        State = x.Saga.CurrentState
                    });
                    return res;
                }));
    }

    public State Completed { get; set; }

    public State Submitted { get; set; }
    public State Canceled { get; set; }
    public State Accepted { get; set; }
    public State Faulted { get; set; }
    public Event<IOrderAccepted> OrderAccepted { get; set; }
    public Event<IOrderSubmitted> OrderSubmitted { set; get; }
    public Event<ICheckOrder> OrderStatusRequested { get; set; }
    public Event<ICustomerAccountClosed> AccountClosed { get; set; }
    public Event<IOrderFulfilmentFaulted> FulfillmentFaulted { get; set; }
    public Event<IOrderFulfilmentCompleted> FulfillmentCompleted { get; set; }
    
    
}
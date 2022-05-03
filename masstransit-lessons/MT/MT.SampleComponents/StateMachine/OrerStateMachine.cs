﻿using System.Net.Mail;
using MassTransit;
using MT.SampleContracts;

namespace MT.SampleComponents.StateMachine;

public class OrerStateMachine : MassTransitStateMachine<OrderState>
{
    public OrerStateMachine()
    {
        Event(() => OrderSubmitted, x => x.CorrelateById(m => m.Message.OrderId));
        //можешь  это пропустить и расказать что надо пистаь для каждого
        Event(() => OrderStatusRequested, x => x.CorrelateById(m => m.Message.OrderId));
        
        
        InstanceState(x => x.CurrentState);

        Initially(
            When(OrderSubmitted)
                .Then(context =>
                    {
                        context.Saga.SubmitDate =context.Message.TimeStamp;
                        context.Saga.CustomerNumber = context.Message.CustomerNumber;
                        context.Saga.Updated = DateTime.UtcNow;
                    }
                )
                .TransitionTo(Submitted)
        );

        //для идемпотенности
        During(Submitted,
            Ignore(OrderSubmitted));
        
        
        // если мы хотим как то дополнить даные потом 
        DuringAny(
            When(OrderSubmitted)
                .Then(context =>
                {
                    context.Saga.SubmitDate ??=context.Message.TimeStamp;
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

    public State Submitted { get; set; }
    public Event<IOrderSubmitted> OrderSubmitted { set; get; }
    public Event<ICheckOrder> OrderStatusRequested { get; set; }
}

public class OrderState : SagaStateMachineInstance, ISagaVersion
{
    public Guid CorrelationId { get; set; }
    public int Version { get; set; }
    public string CurrentState { get; set; }
    public string CustomerNumber { get; set; }
    public DateTime? SubmitDate { get; set; }
    public DateTime? Updated { get; set; }
}
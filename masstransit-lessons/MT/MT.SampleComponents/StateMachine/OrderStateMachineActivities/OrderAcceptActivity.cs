﻿using Automatonymous;
using MassTransit;
using MT.SampleContracts;
using Activity = MassTransit.Courier.Contracts.Activity;

namespace MT.SampleComponents.StateMachine.OrderStateMachineActivities;

public class OrderAcceptActivity :
    IStateMachineActivity<OrderState,IOrderAccepted>
{
    
    /// <summary>
    /// используется для исследования.  провались в TransitionTo.  Смотри на этому примере заполнение данных
    /// </summary>
    /// <param name="context"></param>
    /// <exception cref="NotImplementedException"></exception>
    public void Probe(ProbeContext context)
    {
        context.CreateScope("accept-order");
    }

    public void Accept(StateMachineVisitor visitor)
    {
       visitor.Visit(this);
    }

    public async Task Execute(BehaviorContext<OrderState, IOrderAccepted> context, IBehavior<OrderState, IOrderAccepted> next)
    {
        Console.WriteLine($"OrderAcceptActivity. OrderId {context.Message.OrderId}");
        
        // do something later
        await next.Execute(context).ConfigureAwait(false);
    }

    public async Task Faulted<TException>(BehaviorExceptionContext<OrderState, IOrderAccepted, TException> context, IBehavior<OrderState, IOrderAccepted> next) where TException : Exception
    {
        // можем тут какой то компенсационный евент пульнуть, но пока не хочу
        await next.Faulted(context);
    }
}
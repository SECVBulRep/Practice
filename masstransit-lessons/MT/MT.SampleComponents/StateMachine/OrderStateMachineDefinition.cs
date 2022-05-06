using MassTransit;

namespace MT.SampleComponents.StateMachine;

public class OrderStateMachineDefinition :
    SagaDefinition<OrderState>
{

    public OrderStateMachineDefinition()
    {
        ConcurrentMessageLimit = 3;
    }
    
    protected override void ConfigureSaga(IReceiveEndpointConfigurator endpointConfigurator, ISagaConfigurator<OrderState> sagaConfigurator)
    {
        endpointConfigurator.UseMessageRetry(r=>r.Intervals(500,5000,1000));
        //Эта опция позволяет не отправлять сообщения до того момента, пока текущий шаг не будет выполнен. Если возникнет исключение, то сообщения не отправятся вовсе.
        endpointConfigurator.UseInMemoryOutbox();
        //base.ConfigureSaga(endpointConfigurator, sagaConfigurator);
    }
}
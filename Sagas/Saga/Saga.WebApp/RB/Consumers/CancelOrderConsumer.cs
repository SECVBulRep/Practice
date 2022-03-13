using MassTransit;
using Saga.WebApp.Infra;

namespace Saga.WebApp.RB.Consumers;

public class CancelOrderConsumer :
    IConsumer<IOrderCancelEvent>
{

    private readonly IOrderDataAccess _orderDataAccess;

    public CancelOrderConsumer(IOrderDataAccess orderDataAccess)
    {
        _orderDataAccess = orderDataAccess;
    }

    public async Task Consume(ConsumeContext<IOrderCancelEvent> context)
    {
        _orderDataAccess.DeleteOrder(context.Message.OrderId);
        
    }
}
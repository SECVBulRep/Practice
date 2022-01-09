using MassTransit;

namespace Saga.WebApp.RB.Consumers;

public class CardNumberValidateConsumer :
    IConsumer<IOrderMessage>
{
    public async Task Consume(ConsumeContext<IOrderMessage> context)
    {
        var data = context.Message.CardNumber;
        if (data != "1111222244445555")
        {
            
        }
    }
}
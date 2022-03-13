using MassTransit;

namespace Saga.WebApp.RB.Consumers;

public class CardNumberValidateConsumer :
    IConsumer<ICardValidatorEvent>
{
    public async Task Consume(ConsumeContext<ICardValidatorEvent> context)
    {
        var data = context.Message.CardNumber;
        if (data != "not_ok")
        {
            await context.Publish<IOrderCancelEvent>(
                new {OrderId = context.Message.OrderId, CardNumber = context.Message.CardNumber}
            );
        }
        
    }
}
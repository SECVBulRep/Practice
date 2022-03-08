using MassTransit;

namespace Saga.WebApp.RB.Consumers;

public class CardNumberValidateConsumer :
    IConsumer<ICardValidatorEvent>
{
    public async Task Consume(ConsumeContext<ICardValidatorEvent> context)
    {
        var data = context.Message.CardNumber;
        if (data != "1111222244445555")
        {
            
        }
        
    }
}
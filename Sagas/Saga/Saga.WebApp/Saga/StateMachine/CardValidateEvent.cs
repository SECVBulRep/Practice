using Saga.WebApp.RB;

namespace Saga.WebApp.Saga.StateMachine;

public class CardValidateEvent: ICardValidatorEvent
{
    private readonly OrderStateData orderSagaState;
    public CardValidateEvent(OrderStateData orderStateData)
    {
        this.orderSagaState = orderStateData;
    }

    public Guid OrderId
    {
        get { return orderSagaState.OrderId; }
    }

    public string PaymentCardNumber
    {
        get { return orderSagaState.PaymentCardNumber; }
    }

    public string ProductName
    {
        get { return orderSagaState.ProductName; }
    }
}
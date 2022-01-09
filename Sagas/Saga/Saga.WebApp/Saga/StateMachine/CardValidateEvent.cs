using Saga.WebApp.RB;

namespace Saga.WebApp.Saga.StateMachine;

public class CardValidateEvent: ICardValidatorEvent
{
    private readonly OrderStateData orderSagaState;
    public CardValidateEvent(OrderStateData orderStateData)
    {
        this.orderSagaState = orderStateData;
    }

    public Guid OrderId => orderSagaState.OrderId;
    public string PaymentCardNumber => orderSagaState.PaymentCardNumber;
    public string ProductName => orderSagaState.ProductName;
}
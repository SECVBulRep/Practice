using MassTransit;

namespace MT.SampleComponents.CourierAcitivities;

public class PaymentActivity :
    IActivity<IPaymentArguments, IPaymentLog>
{
    public async Task<ExecutionResult> Execute(ExecuteContext<IPaymentArguments> context)
    {
        string cardNumber = context.Arguments.CardNumber;

        //что бы запустился шедуллер
        await Task.Delay(1500);
        
        if (string.IsNullOrEmpty(cardNumber))
            throw new ArgumentNullException(nameof(cardNumber));

        if (cardNumber.StartsWith("5999"))
            throw new InvalidOperationException("the card number is invalid");

        await Task.Delay(500);

        return context.Completed(new
        {
            AuthorizationCode = "77777"
        });
    }

    public async Task<CompensationResult> Compensate(CompensateContext<IPaymentLog> context)
    {
        await Task.Delay(500);
        return context.Compensated();
    }
}
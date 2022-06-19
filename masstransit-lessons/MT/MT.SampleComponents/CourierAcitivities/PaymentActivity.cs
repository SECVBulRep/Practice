using MassTransit;

namespace MT.SampleComponents.CourierAcitivities;

public class PaymentActivity :
    IActivity<IPaymentArguments, IPaymentLog>
{
    public async Task<ExecutionResult> Execute(ExecuteContext<IPaymentArguments> context)
    {
        string cardNumber = context.Arguments.CardNumber;

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

public interface IPaymentArguments
{
    Guid OrderId { get; set; }
    decimal Amount { get; set; }
    string CardNumber { get; set; }
}

public interface IPaymentLog
{
    string AuthorizationCode { get; set; }
}
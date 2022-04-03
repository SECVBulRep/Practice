using System.Net.Mail;
using MassTransit;
using Microsoft.Extensions.Logging;
using MT.SampleContracts;

namespace MT.SampleComponents.Consumers;

public class SubmitOrderConsumer
    : IConsumer<ISubmitOrder>
{
    private ILogger<SubmitOrderConsumer> _logger;

    public SubmitOrderConsumer(ILogger<SubmitOrderConsumer> logger)
    {
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<ISubmitOrder> context)
    {
        _logger.Log(LogLevel.Debug,"SubmitOrderConsumer: {CustomerNumber}", context.Message.CustomerNumber);
        
        if (context.Message.CustomerNumber.Contains("Test"))
        {
            await context.RespondAsync<IOrderSubmissionRejected>(new
            {
                OrderId = context.Message.OrderId,
                TimeStamp = context.Message.TimeStamp,
                CustomerNumber = context.Message.CustomerNumber,
                Reason ="Причина test customer"
            });
            return;
        }

        await context.RespondAsync<IOrderSubmissionAccepted>(new
        {
            OrderId = context.Message.OrderId,
            TimeStamp = context.Message.TimeStamp,
            CustomerNumber = context.Message.CustomerNumber,
        });
    }
}
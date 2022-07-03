using MassTransit;
using MassTransit.Courier.Contracts;
using MT.SampleContracts;

namespace MT.SampleComponents.Consumers;

public class FulfillOrderConsumer : IConsumer<IFulfillOrder>
{
    public async Task Consume(ConsumeContext<IFulfillOrder> context)
    {
        if (context.Message.CustomerNumber == "INVALID")
        {
            throw new InvalidOperationException("We tried, but customer is invalid");
        }
        
        //для message retry
        if (context.Message.CustomerNumber == "MAYBE")
        {
            if(new Random().Next(100)>50)
                throw new ApplicationException("Randomly exploded ");
        }
        
        
        var builder = new RoutingSlipBuilder(Guid.NewGuid());
        // обрати внимание на нижнее подчеркивание в наименовании!!!

        builder.AddVariable("OrderId", context.Message.OrderId);

        builder.AddActivity("AllocateInventory", new Uri("queue:allocate-inventory_execute"), new
        {
            ItemNumber = "ITEM123",
            Quantity = 10
        });

        builder.AddActivity("PaymentActivity", new Uri("queue:payment_execute"), new
        {
            CardNumber = context.Message.PaymentCardNumber ?? "5999-1111-1111-1111",
            Amount = 10m
        });

        await builder.AddSubscription(context.SourceAddress, RoutingSlipEvents.Faulted | RoutingSlipEvents.Supplemental,
            RoutingSlipEventContents.None,
            x => x.Send<IOrderFulfilmentFaulted>(new
            {
                OrderId = context.Message.OrderId
            }));

        await builder.AddSubscription(context.SourceAddress, RoutingSlipEvents.Completed | RoutingSlipEvents.Supplemental,
            RoutingSlipEventContents.None,
            x => x.Send<IOrderFulfilmentCompleted>(new
            {
                OrderId = context.Message.OrderId
            }));
        
        
        var routingSlip = builder.Build();

        await context.Execute(routingSlip);
    }
}
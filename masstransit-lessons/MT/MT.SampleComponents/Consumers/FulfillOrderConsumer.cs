using MassTransit;
using MT.SampleContracts;

namespace MT.SampleComponents.Consumers;

public class FulfillOrderConsumer: IConsumer<IFulfillOrder>
{
    public async Task Consume(ConsumeContext<IFulfillOrder> context)
    {
        var builder = new RoutingSlipBuilder(Guid.NewGuid());
        // обрати внимание на нижнее подчеркивание в наименовании!!!
        
        builder.AddVariable("OrderId",context.Message.OrderId);
        
        builder.AddActivity("AllocateInventory",new Uri("queue:allocate-inventory_execute"),new
        {
            ItemNumber = "ITEM123",
            Quantity = 10
        });
        
        builder.AddActivity("PaymentActivity",new Uri("queue:payment_execute"),new
        {
            CardNumber = "5999-1111-1111-1111",
            Amount = 10m
        });
        
        
        var routingSlip = builder.Build();

        await context.Execute(routingSlip);
    }
}
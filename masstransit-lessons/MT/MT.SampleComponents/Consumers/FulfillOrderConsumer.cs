using MassTransit;
using MT.SampleContracts;

namespace MT.SampleComponents.Consumers;

public class FulfillOrderConsumer: IConsumer<IFulfillOrder>
{
    public async Task Consume(ConsumeContext<IFulfillOrder> context)
    {
        var builder = new RoutingSlipBuilder(Guid.NewGuid());
        // обрати внимание на нижнее подчеркивание в наименовании!!!
        builder.AddActivity("AllocateInventory",new Uri("queue:allocate-inventory_execute"),new
        {
            ItemNumber = "ITEM123",
            Quantity = 10.0m
        });
        
        builder.AddVariable("OrderId",context.Message.OrderId);

        var routingSlip = builder.Build();

        await context.Execute(routingSlip);
    }
}
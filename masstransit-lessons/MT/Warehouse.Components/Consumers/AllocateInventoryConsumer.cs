using MassTransit;
using Warehouse.Contracts;

namespace Warehouse.Components.Consumers;

public class AllocateInventoryConsumer : IConsumer<IAllocateInventory>
{
    public async Task Consume(ConsumeContext<IAllocateInventory> context)
    {
        await Task.Delay(500);
        await context.RespondAsync<IInventoryAllocated>(new
        {
            AllocationId = context.Message.AllocationId,
            ItemNumber = context.Message.ItemNumber,
            Quantity = context.Message.Quantity,
        });
    }
}
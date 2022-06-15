﻿using MassTransit;
using Warehouse.Contracts;

namespace MT.SampleComponents.CourierAcitivities;

public class AllocateInventoryActivity : IActivity<IAllocateInventoryArguments, AllocateInventoryLog>
{
    private readonly IRequestClient<IAllocateInventory> _requestClient;

    public AllocateInventoryActivity(IRequestClient<IAllocateInventory> requestClient)
    {
        _requestClient = requestClient;
    }

    public async Task<ExecutionResult> Execute(ExecuteContext<IAllocateInventoryArguments> context)
    {
        var orderId = context.Arguments.OrderId;
        var itemNumber = context.Arguments.ItemNumber;
        if (string.IsNullOrEmpty(itemNumber))
            throw new ArgumentException(nameof(itemNumber));
        var quantity = context.Arguments.Quantity;
        if (quantity <= 0)
            throw new ArgumentException(nameof(quantity));

        var allocationId = Guid.NewGuid();

        var response = await _requestClient.GetResponse<IInventoryAllocated>(new
        {
            AlocationId = allocationId,
            ItemNumber = itemNumber,
            Quantity = quantity
        });


        return context.Completed(new
        {
            AlocationId = allocationId
        });
    }

    public async Task<CompensationResult> Compensate(CompensateContext<AllocateInventoryLog> context)
    {
        await context.Publish<IAllocationReleaseRequested>(new
        {
            AlocationId = context.Log.AllocationId,
            Reason = "order faulted"
        });

        return context.Compensated();
    }
}
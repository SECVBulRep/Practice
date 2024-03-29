﻿using MassTransit;
using Warehouse.Contracts;

namespace MT.SampleComponents.CourierAcitivities;

public class AllocateInventoryActivity : IActivity<IAllocateInventoryArguments, IAllocateInventoryLog>
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
        int quantity = context.Arguments.Quantity;
        if (quantity<1)
            throw new ArgumentException(nameof(quantity));

        var allocationId = Guid.NewGuid();

        var response = await _requestClient.GetResponse<IInventoryAllocated>(new
        {
            AllocationId = allocationId,
            ItemNumber = itemNumber,
            Quantity = quantity
        });

        return context.Completed(new
        {
            AllocationId = allocationId
        });
    }

    public async Task<CompensationResult> Compensate(CompensateContext<IAllocateInventoryLog> context)
    {
        await context.Publish<IAllocationReleaseRequested>(new
        {
            AllocationId = context.Log.AllocationId,
            Reason = "order faulted"
        });

        return context.Compensated();
    }
}
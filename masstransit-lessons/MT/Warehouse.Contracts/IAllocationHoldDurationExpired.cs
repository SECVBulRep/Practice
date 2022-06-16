namespace Warehouse.Contracts;

public interface IAllocationHoldDurationExpired
{
    Guid AllocationId { get; set; }
}
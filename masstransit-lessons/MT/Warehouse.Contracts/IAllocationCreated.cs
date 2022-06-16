namespace Warehouse.Contracts;

public interface IAllocationCreated
{
    Guid AllocationId { get; set; }
    TimeSpan HoldDuration { get; set; }
}
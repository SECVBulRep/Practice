namespace Warehouse.Contracts;

public interface IAllocationReleaseRequested
{
    Guid AllocationId { get; set; }
    string Reason { get; set; }
}
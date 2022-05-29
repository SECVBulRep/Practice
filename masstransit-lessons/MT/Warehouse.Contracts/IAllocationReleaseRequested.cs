namespace Warehouse.Contracts;

public interface IAllocationReleaseRequested
{
    Guid AlocationId { get; set; }
    string Reason { get; set; }
}
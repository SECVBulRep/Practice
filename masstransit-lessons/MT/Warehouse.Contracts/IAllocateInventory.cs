namespace Warehouse.Contracts;

public interface IAllocateInventory
{
    Guid AllocationId { get; set; }
    string ItemNumber { get; set; }
    int Quantity { get; set; }
}
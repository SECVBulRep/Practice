namespace Warehouse.Contracts;

public interface IAllocateInventory
{
    Guid AlocationId { get; set; }
    string ItemNumber { get; set; }
    decimal Quantity { get; set; }
}
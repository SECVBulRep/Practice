namespace Warehouse.Contracts;

public interface IInventoryAllocated
{
    Guid AlocationId { get; set; }
    string ItemNumber { get; set; }
    decimal Quantity { get; set; } 
}
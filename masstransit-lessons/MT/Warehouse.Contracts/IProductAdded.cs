namespace Warehouse.Contracts;

public interface IProductAdded
{
    Guid ProductId { get; }
    DateTime Timestamp { get; }
    string ManufacturerId { get; }
    string Name { get; }
}
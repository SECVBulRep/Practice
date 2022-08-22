namespace Delivery.Contracts;

public interface IСurrierVisited
{
    Guid СurrierId { get; }
    DateTime Entered { get; }
    DateTime Left { get; }
}
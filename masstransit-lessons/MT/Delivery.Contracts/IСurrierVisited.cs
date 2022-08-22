namespace Delivery.Contracts;

public interface ICurrierVisited
{
    Guid СurrierId { get; }
    DateTime Entered { get; }
    DateTime Left { get; }
}


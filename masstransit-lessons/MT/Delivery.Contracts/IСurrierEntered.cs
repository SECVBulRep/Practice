namespace Delivery.Contracts;

public interface IСurrierEntered
{
    Guid СurrierId { get; }
    DateTime Timestamp { get; }
}
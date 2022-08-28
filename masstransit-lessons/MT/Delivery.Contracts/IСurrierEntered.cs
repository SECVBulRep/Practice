namespace Delivery.Contracts;

public interface IСurrierEntered
{
    Guid СurrierId { get; set; }
    DateTime Timestamp { get; set; }
}
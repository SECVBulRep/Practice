namespace Delivery.Contracts;

public interface ICurrierLeft
{
    Guid CurrierId { get; }
    DateTime Timestamp { get; }
}
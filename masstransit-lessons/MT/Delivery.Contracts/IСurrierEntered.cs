namespace Delivery.Contracts;

public interface ICurrierEntered
{
    Guid CurrierId { get; set; }
    DateTime Timestamp { get; set; }
}
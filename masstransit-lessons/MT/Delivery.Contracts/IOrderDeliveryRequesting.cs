namespace Delivery.Contracts;

public interface IOrderDeliveryRequesting
{
    Guid OrderId { get; }
}
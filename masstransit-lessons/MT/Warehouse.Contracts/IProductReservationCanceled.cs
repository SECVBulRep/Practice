namespace Warehouse.Contracts;

public interface IProductReservationCanceled
{
    public Guid ProductId { get; set; }
}
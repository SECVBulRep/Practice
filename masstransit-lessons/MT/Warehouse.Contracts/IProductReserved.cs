namespace Warehouse.Contracts;

public interface IProductReserved
{
    public Guid ReservationId { get; set; }

    public Guid ClientId { get; set; }

    public DateTime TimeStamp { get; set; }

    public Guid ProductId { get; set; }
}
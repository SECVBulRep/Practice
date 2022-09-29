namespace Warehouse.Contracts;

public interface IReservationExpired
{
    public Guid ReservationId { get; set; }
}
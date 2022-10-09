namespace Warehouse.Contracts;

public interface IReservationCancelationRequested
{
    public Guid ReservationId { get; set; }
    public DateTime TimeStamp { get; set; }
}
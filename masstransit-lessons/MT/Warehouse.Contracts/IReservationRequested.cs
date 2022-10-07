namespace Warehouse.Contracts;

public interface IReservationRequested
{
    public Guid ReservationId { get; set; }

    public Guid ClientId { get; set; }

    public DateTime TimeStamp { get; set; }
    public TimeSpan? Duration { get; set; }
    public Guid ProductId { get; set; }
}
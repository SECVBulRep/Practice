namespace MT.SampleContracts;

public interface IOrderFulfilmentCompleted
{
    public Guid OrderId { get; set; }

    DateTime Stamp { get; set; }

}
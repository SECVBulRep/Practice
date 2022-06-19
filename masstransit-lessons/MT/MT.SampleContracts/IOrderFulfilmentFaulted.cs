namespace MT.SampleContracts;

public interface IOrderFulfilmentFaulted
{
    public Guid OrderId { get; set; }

    DateTime Stamp { get; set; }

}
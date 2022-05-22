namespace MT.SampleContracts;

public interface IOrderAccepted
{
    Guid OrderId { get; set; }
    DateTime TimeStamp { get; set; }
}
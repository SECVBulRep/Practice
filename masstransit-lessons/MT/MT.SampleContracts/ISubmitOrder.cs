namespace MT.SampleContracts;

public interface ISubmitOrder
{
    Guid OrderId { get; set; }
    DateTime TimeStamp { get; set; }
    string CustomerNumber { get; set; }
}
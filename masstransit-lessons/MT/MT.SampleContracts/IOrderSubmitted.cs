namespace MT.SampleContracts;

public interface IOrderSubmitted
{
    Guid OrderId { get; set; }
    DateTime TimeStamp { get; set; }
    string CustomerNumber { get; set; }
}
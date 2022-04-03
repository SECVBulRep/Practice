namespace MT.SampleContracts;

public interface IOrderSubmissionRejected
{
    Guid OrderId { get; set; }
    DateTime TimeStamp { get; set; }
    string CustomerNumber { get; set; }
    string Reason { get; set; }
}
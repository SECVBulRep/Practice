namespace MT.SampleContracts;

public interface IOrderSubmissionAccepted
{
    Guid OrderId { get; set; }
    DateTime TimeStamp { get; set; }
    string CustomerNumber { get; set; }
    
}
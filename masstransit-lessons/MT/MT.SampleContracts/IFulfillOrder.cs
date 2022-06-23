namespace MT.SampleContracts;

public interface IFulfillOrder
{
    public Guid OrderId { get; set; }
    string PaymentCardNumber { get; set; }
    string CustomerNumber { get; set; }
    
}
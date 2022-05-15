namespace MT.SampleContracts;

public interface ICustomerAccountClosed
{
    Guid CustomerId { get; set; }
    string CustomerNumber { get; set; }
    DateTime Stamp { get; set; }
}
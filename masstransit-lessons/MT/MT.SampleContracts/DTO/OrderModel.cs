namespace MT.SampleContracts.DTO;

public class OrderModel
{
    public Guid Id { get; set; }
    public string CustomerId { get; set; }
    public string PaymentCardNumber { get; set; }
}
namespace Saga.WebApp.RB;

public class IOrderCancelEvent 
{
    public Guid OrderId { get; set; }
    public string ProductName { get; set; }
    public string CardNumber { get; set; }
}
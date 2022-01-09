namespace Saga.WebApp.RB;

public class IOrderStartEvent
{
    public Guid OrderId { get; set; }
    public string ProductName { get; set; }
    public string CardNumber { get; set; }
}
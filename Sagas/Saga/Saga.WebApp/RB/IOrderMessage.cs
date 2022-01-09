namespace Saga.WebApp.RB;

public interface IOrderMessage
{
    public Guid OrderId { get; set; }
    public string ProductName { get; set; }
    public string CardNumber { get; set; }
}
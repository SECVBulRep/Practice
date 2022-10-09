namespace Warehouse.Contracts;

public interface IProductCheckedOut
{
    public Guid ClientId { get; set; }
    public DateTime TimeStamp { get; set; }
    public Guid ProductId { get; set; }
}
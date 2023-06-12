namespace WM.Microservices.Delivery.Api.Models;

public class ProductInOrder {

    public Order Order { get; set; }
    
    public int  OrderId { get; set; }
    
    public Order Product { get; set; }
    
    public int  ProductId { get; set; }
}
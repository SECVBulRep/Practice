using System.ComponentModel.DataAnnotations;

namespace WM.Microservices.Delivery.Api.Models;

public class Order
{
    [Key]
    [Required]
    public int Id { get; set; }

    [Required]
    public string Description { get; set; } = null!;

    public IList<ProductInOrder> ProductsInOrder { get; set; }
}
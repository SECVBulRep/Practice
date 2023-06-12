using System.ComponentModel.DataAnnotations;

namespace WM.Microservices.Delivery.Api.Models;

public class Product
{
    [Key]
    [Required]
    public int Id { get; set; }

    [Required]
    public string Name { get; set; } = null!;
   
    // public int OrderId { get; set; }
    // public Order Order { get; set; } = null!;
}
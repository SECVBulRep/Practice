using System.ComponentModel.DataAnnotations;

namespace WM.Microservices.Shop.Api.Models;

public class Product
{
    [Key]
    public int Id { get; set; }
   
    [Required]
    public string Name { get; set; } = null!;
    
    [Required]
    public string Description { get; set; } = null!;

    [Required]
    public decimal Price { get; set; }
}
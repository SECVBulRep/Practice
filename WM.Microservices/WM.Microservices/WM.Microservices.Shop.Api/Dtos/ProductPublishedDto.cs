namespace WM.Microservices.Shop.Api.Dtos;

public class ProductPublishedDto
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;
    
    public string Event { get; set; } = null!;
}
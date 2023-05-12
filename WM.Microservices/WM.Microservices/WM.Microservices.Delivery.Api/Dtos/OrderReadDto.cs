namespace WM.Microservices.Delivery.Api.Dtos;

public class OrderReadDto
{
    public int Id { get; set; }
   
    public string Description { get; set; } = null!;
}